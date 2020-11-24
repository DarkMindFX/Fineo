using Fineo.Interfaces;
using System.Collections.Generic;

namespace Fineo.FileStorage.AzureBlob
{
    public class FileStorageParams : IFileStorageParams
    {
        public FileStorageParams()
        {
            Parameters = new Dictionary<string, string>();
            Parameters["StorageAccountName"] = null;
            Parameters["StorageAccountKey"] = null;
            Parameters["ContainerName"] = null;
            Parameters["BlobEndpoint"] = null;
        }
        public Dictionary<string, string> Parameters 
        {
            get;
            set;
        }
    }
}
