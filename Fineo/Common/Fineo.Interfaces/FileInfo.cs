using System;

namespace Fineo.Interfaces
{
    public class FileInfo
    {
        public string Path
        {
            get;
            set;
        }

        public long Size
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }
    }
}
