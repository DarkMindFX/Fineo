using System;
using System.Collections.Generic;
using System.Text;

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

        FileInfo Download(FileInfo fileInfo);

        bool Upload(FileInfo fileInfo);

        IFileStorageParams CreateParams();
    }
}
