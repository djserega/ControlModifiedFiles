using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    public class RowFilter
    {
        public string Present { get; set; }
        public string Filter { get; set; }

        public RowFilter(string present, string filter)
        {
            Present = present;
            Filter = filter;
        }
    }
}
