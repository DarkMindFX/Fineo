using System;
using System.Collections.Generic;
using System.Text;

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
