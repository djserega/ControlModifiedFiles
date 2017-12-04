using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    public class ColumnAttribute : Attribute
    {
        #region Properties

        public string HeaderName { get; set; }
        public bool VisibleColumn { get; set; }
        public string SortMemberPath { get; set; }
        public ListSortDirection? SortDirection { get; set; }
        public bool IsOnlyRead { get; set; }

        #endregion

        public ColumnAttribute(string headerName, bool visibleColumn = true,
            string sortMemberPath = "", DataGridSortDirection sortDirection = DataGridSortDirection.none,
            bool isOnlyRead = false)
        {
            HeaderName = headerName;
            VisibleColumn = visibleColumn;
            SortMemberPath = sortMemberPath;
            switch (sortDirection)
            {
                case DataGridSortDirection.asc:
                    SortDirection = ListSortDirection.Ascending;
                    break;
                case DataGridSortDirection.desc:
                    SortDirection = ListSortDirection.Descending;
                    break;
                default:
                    SortDirection = null;
                    break;
            }
            IsOnlyRead = isOnlyRead;
        }
    }
}