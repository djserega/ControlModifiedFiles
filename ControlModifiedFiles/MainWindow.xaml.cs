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
        #region Properties

        public ICollection<FileSubscriber> _listFile = new List<FileSubscriber>();
        private Subscriber subscriber = new Subscriber();

        #endregion

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
            List<FileSubscriber> list = _listFile.ToList();

            string path = new DirFile().GetFileChecked(this);

            if (String.IsNullOrWhiteSpace(path))
            {
                dgList.ItemsSource = list;
                return;
            }

            DirFile pathInfo = new DirFile(path);

            ulong sizeFile = pathInfo.GetFileSize();

            FileSubscriber fileChecked = new FileSubscriber()
            {
                Checked = true,
                Path = path,
                Size = sizeFile,
                SizeString = pathInfo.GetSizeFormat(sizeFile)
            };

            subscriber.SubscribeChangeFile(fileChecked);

            list.Add(fileChecked);

            SetItemSouce(list);
        }

        private void MiDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            FileSubscriber selectedRow = (FileSubscriber)dgList.CurrentItem;
            if (selectedRow == null)
                return;

            List<FileSubscriber> list = _listFile.ToList();

            subscriber.UnsubscribeChangeFile(selectedRow);

            list.Remove(selectedRow);

            SetItemSouce(list);
        }

        private void MiSaveTable_Click(object sender, RoutedEventArgs e)
        {
            bool result = new SaveLoadConfig().SaveConfig(_listFile.ToList());
        }

        private void MiLoadTable_Click(object sender, RoutedEventArgs e)
        {
            List<FileSubscriber> list = new SaveLoadConfig().LoadConfig();
            if (list != null)
            {
                SetItemSouce(list);
                subscriber.SubscribeChangeFiles(list);
            }
        }

        #endregion

        #region DataGridList

        private void DgList_CurrentCellChanged(object sender, EventArgs e)
        {
            //FileSubscriber file = (FileSubscriber)dgList.CurrentItem;
            //if (file != null)
            //{
            //    if (file.Checked)
            //        subscriber.SubscribeChangeFile(file);
            //    else
            //        subscriber.UnsubscribeChangeFile(file);
            //}
        }

        private void DgList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            FileSubscriber file = (FileSubscriber)dgList.CurrentItem;

        }

        #endregion

        #region Private methods

        private void SetItemSouce(List<FileSubscriber> list)
        {
            _listFile = list;
            dgList.ItemsSource = _listFile;
        }

        #endregion
    }
}
