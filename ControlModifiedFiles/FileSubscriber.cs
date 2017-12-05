using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ControlModifiedFiles
{
    public class FileSubscriber
    {
        [Column("Выбрано")]
        public bool Checked { get; set; }
        [Column("Путь к файлу", IsOnlyRead = true)]
        public string Path { get; set; }
        [Column("Размер (byte)", IsOnlyRead = true)]
        public ulong Size { get; set; }
        [Column("Размер", SortMemberPath = "Size", IsOnlyRead = true)]
        public string SizeString { get; set; }
        [Column("№ версии",IsOnlyRead = true)]
        public int Version { get; set; }
        [Column("Каталог версий", IsOnlyRead = true)]
        public string DirectoryVersion { get; set; }
    }
}
