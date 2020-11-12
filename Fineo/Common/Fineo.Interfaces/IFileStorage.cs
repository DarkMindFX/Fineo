using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fineo.Interfaces
{
    public interface IFileStorageParams
    {
        Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    public interface IFileStorage
    {
        void Init(IFileStorageParams initParams);

        Task<FileInfo> DownloadAsync(FileInfo fileInfo);

        Task<FileInfo> GetFileInfo(FileInfo fileInfo);

        Task<List<FileInfo>> GetFolderContent(FileInfo fileInfo);

        Task<bool> UploadAsync(FileInfo fileInfo);

        Task<bool> DeleteAsync(FileInfo fileInfo);

        IFileStorageParams CreateParams();
    }
}
