using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    public class FileSubscriber
    {
        public bool Checked { get; set; }
        public string Path { get; set; }
        public ulong Size { get; set; }
        public string SizeString { get; set; }
        public int Version { get; set; }
        public string DirectoryVersion { get; set; }
    }
}
