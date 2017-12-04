using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ICollection<FileInfo> _listFile = new List<FileInfo>();
        private Subscriber subscriber = new Subscriber();

        #region Window

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgList.ItemsSource = _listFile;
        }

        #endregion

        #region MainMenu

        private void MiAddFile_Click(object sender, RoutedEventArgs e)
        {
            List<FileInfo> list = _listFile.ToList();

            string path = new DirFile().GetFileChecked(this);

            if (String.IsNullOrWhiteSpace(path))
            {
                dgList.ItemsSource = list;
                return;
            }

            DirFile pathInfo = new DirFile(path);

            ulong sizeFile = pathInfo.GetFileSize();

            FileInfo fileChecked = new FileInfo()
            {
                //Checked = true,
                Path = path,
                Size = sizeFile,
                SizeString = pathInfo.GetSizeFormat(sizeFile)
            };

            list.Add(fileChecked);

            SetItemSouce(list);
        }

        private void MiDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            FileInfo selectedRow = (FileInfo)dgList.CurrentItem;
            if (selectedRow == null)
                return;

            List<FileInfo> list = _listFile.ToList();

            list.Remove(selectedRow);

            SetItemSouce(list);
        }

        private void MiSaveTable_Click(object sender, RoutedEventArgs e)
        {
            bool result = new SaveLoadConfig().SaveConfig(_listFile.ToList());
        }

        private void MiLoadTable_Click(object sender, RoutedEventArgs e)
        {
            List<FileInfo> list = new SaveLoadConfig().LoadConfig();
            if (list != null)
                SetItemSouce(list);
        }

        #endregion

        private void SetItemSouce(List<FileInfo> list)
        {
            _listFile = list;
            dgList.ItemsSource = _listFile;
        }

        private void DgList_CurrentCellChanged(object sender, EventArgs e)
        {
            FileInfo dd = (FileInfo)dgList.CurrentItem;
            
        }
    }
}
