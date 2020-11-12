using Fineo.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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
                                        blob.Properties.LastModified.Value.UtcDateTime  : DateTime.MinValue;
                
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
            await using (var sw = new System.IO.StreamWriter(blob.OpenWriteAsync().Result))
            {
                sw.Write(Encoding.Unicode.GetChars(fileInfo.Content));
                result = true;
            }

            return result;
        }

        #region Support methods

        CloudStorageAccount createCloudStorageAccount(IFileStorageParams fsParams)
        {
            String accountName = (string)fsParams.Parameters["StorageAccountName"];
            String accountKey = (string)fsParams.Parameters["StorageAccountKey"];
            String blobEndpoint = (string)fsParams.Parameters["BlobEndpoint"];
            String connString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};", accountName, accountKey)
                                + (!string.IsNullOrEmpty(blobEndpoint) ? "BlobEndpoint=" + blobEndpoint : string.Empty);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            return storageAccount;
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
