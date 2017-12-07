using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    public class RowFilter
    {
        [Column("Представление")]
        public string Present { get; set; }
        [Column("Формат фильтра")]
        public string Filter { get; set; }

        public RowFilter() { }

        public RowFilter(string present, string filter)
        {
            Present = present;
            Filter = filter;
        }
    }
}
