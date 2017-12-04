using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    public class FileInfo
    {
        public bool Checked { get; set; }
        public string Path { get; set; }
        public ulong Size { get; set; }
        public string SizeString { get; set; }
        public int Version { get; set; }
    }
}
