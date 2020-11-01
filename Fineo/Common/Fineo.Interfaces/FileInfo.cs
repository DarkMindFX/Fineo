using System;
using System.Collections.Generic;
using System.Text;

namespace Fineo.Interfaces
{
    public class FileInfo
    {
        string Path
        {
            get;
            set;
        }

        long Size
        {
            get;
            set;
        }

        byte[] Content
        {
            get;
            set;
        }
    }
}
