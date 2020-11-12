using Azure.Storage.Blobs;
using Fineo.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fineo.FileStorage.AzureBlob
{

    public class FileStorage : IFileStorage
    {
        IFileStorageParams initParams = null;
        CloudBlobContainer container = null;

        public IFileStorageParams CreateParams()
        {
            return new FileStorageParams();
        }

        public async Task<FileInfo> DownloadAsync(FileInfo fileInfo)
        {
            FileInfo result = new FileInfo();

            CloudBlob blob = container.GetBlobReference(fileInfo.Path);
            if (blob != null)
            {
                await blob.FetchAttributesAsync();
                result.Path = fileInfo.Path;
                result.Size = blob.Properties.Length;
                result.LastModified = blob.Properties.LastModified != null && blob.Properties.LastModified.HasValue ?
                                        blob.Properties.LastModified.Value.UtcDateTime : DateTime.MinValue;

                result.Content = new byte[result.Size];
                await blob.DownloadToByteArrayAsync(result.Content, 0);
            }

            return result;
        }

        public void Init(IFileStorageParams initParams)
        {
            this.initParams = initParams;

            CloudStorageAccount account = createCloudStorageAccount(initParams);
            CloudBlobClient client = account.CreateCloudBlobClient();

            container = getBlobContainer(client, this.initParams.Parameters["ContainerName"]);
        }

        public async Task<bool> UploadAsync(FileInfo fileInfo)
        {
            bool result = false;

            CloudBlockBlob blob = container.GetBlockBlobReference(fileInfo.Path);
            await using (var sw = new System.IO.BinaryWriter(blob.OpenWriteAsync().Result))
            {
                sw.Write(fileInfo.Content);
                result = true;
            }

            return result;
        }

        public async Task<bool> DeleteAsync(FileInfo fileInfo)
        {
            bool result = false;

            CloudBlockBlob blob = container.GetBlockBlobReference(fileInfo.Path);

            await Task.Run(() =>
                {
                    var t = blob.DeleteIfExistsAsync();
                    result = t.Result;
                }
            );

            return result;
        }

        public async Task<FileInfo> GetFileInfo(FileInfo fileInfo)
        {
            FileInfo result = new FileInfo();

            CloudBlob blob = container.GetBlobReference(fileInfo.Path);
            if (blob != null)
            {
                await blob.FetchAttributesAsync();
                result.Path = fileInfo.Path;
                result.Size = blob.Properties.Length;
                result.LastModified = blob.Properties.LastModified != null && blob.Properties.LastModified.HasValue ?
                                        blob.Properties.LastModified.Value.UtcDateTime : DateTime.MinValue;


            }

            return result;
        }

        public async Task<List<FileInfo>> GetFolderContent(FileInfo fileInfo)
        {
            List<FileInfo> result = null;

            var blobList = container.ListBlobsSegmentedAsync(fileInfo.Path, false, BlobListingDetails.None, int.MaxValue, null, null, null);
            if (blobList != null)
            {
                result = new List<FileInfo>();
                var blobResultSegment = blobList.Result;
                foreach (var d in blobResultSegment.Results)
                {
                    CloudBlobDirectory dir = d as CloudBlobDirectory;
                    BlobContinuationToken blobContinuationToken = null;
                    do
                    {
                        var resultSegment = await dir.ListBlobsSegmentedAsync(
                            useFlatBlobListing: true,
                            blobListingDetails: BlobListingDetails.None,
                            maxResults: null,
                            currentToken: blobContinuationToken,
                            options: null,
                            operationContext: null
                        );

                        // Get the value of the continuation token returned by the listing call.
                        blobContinuationToken = resultSegment.ContinuationToken;

                        result.AddRange(resultSegment.Results.Select( x => new FileInfo() { Path = x.Uri.ToString().Replace(dir.Uri.ToString(), string.Empty) } ));

                    } while (blobContinuationToken != null);

                }
            }

            return result;
        }

        #region Support methods

        CloudStorageAccount createCloudStorageAccount(IFileStorageParams fsParams)
        {
            string connString = GetConnectionString(fsParams);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            return storageAccount;
        }

        string GetConnectionString(IFileStorageParams fsParams)
        {
            string accountName = (string)fsParams.Parameters["StorageAccountName"];
            string accountKey = (string)fsParams.Parameters["StorageAccountKey"];
            string blobEndpoint = (string)fsParams.Parameters["BlobEndpoint"];
            string connString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};", accountName, accountKey)
                                + (!string.IsNullOrEmpty(blobEndpoint) ? "BlobEndpoint=" + blobEndpoint : string.Empty);

            return connString;
        }

        CloudBlobContainer getBlobContainer(CloudBlobClient blobClient, String name)
        {
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(name);

            blobContainer.CreateIfNotExistsAsync().Wait();

            return blobContainer;
        }



        #endregion
    }
}
