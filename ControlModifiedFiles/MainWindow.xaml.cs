﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            if (Properties.Settings.Default.Autoload)
                LoadTable();

            StartAutoupdateVersionAsync();
        }

        #endregion

        #region MainMenu

        private void MiAddFile_Click(object sender, RoutedEventArgs e)
        {
            List<FileSubscriber> list = _listFile.ToList();

            string path = new DirFile().GetFileChecked(this);

            if (String.IsNullOrWhiteSpace(path))
            {
                SetItemSouce(list);
                return;
            }

            FileSubscriber finded = list.Find(f => f.Path == path);

            if (finded != null)
            {
                Dialog.ShowMessage($"Выбранный файл уже контролируется:\n" +
                    $"{path}");
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
            LoadTable();
        }

        private void MiSettings_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
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

        #region Update version

        private async Task StartUpdateVersionFiles()
        {
            List<FileSubscriber> listVersion = await subscriber.LoadVersionFilesAsync(_listFile.ToList());

            dgList.IsReadOnly = true;

            List<FileSubscriber> list = _listFile.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                FileSubscriber finded = listVersion.Find(f => f.Path == list[i].Path);
                if (finded != null)
                    list[i].Version = finded.Version;
                else
                    list[i].Version = 0;
            }

            SetItemSouce(list);

            dgList.IsReadOnly = false;
        }

        private async void StartAutoupdateVersionAsync()
        {
            if (Properties.Settings.Default.AutoupdateVersion)
            {
                await StartUpdateVersionFiles();
                await Task.Delay(5 * 1000);
                StartAutoupdateVersionAsync();
            }
        }

        #endregion

        #region Private methods

        private void LoadTable()
        {
            List<FileSubscriber> list = new SaveLoadConfig().LoadConfig();
            if (list != null)
            {
                subscriber.SubscribeChangeFiles(list);
                StartUpdateVersionFiles();
                SetItemSouce(list);
            }
        }

        private void SetItemSouce(List<FileSubscriber> list)
        {
            _listFile = list;
            dgList.ItemsSource = _listFile;
        }

        #endregion

        private void DgList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            foreach (PropertyInfo propInfo in new FileSubscriber().GetType().GetProperties())
            {
                DataGridColumn column = dgList.Columns.FirstOrDefault(f => (string)f.Header == propInfo.Name);
                if (column != null)
                {
                    var propAttribute = propInfo.GetCustomAttributes<ColumnAttribute>();
                    if (propAttribute.Count() > 0)
                    {
                        var attribute = propAttribute.First();

                        column.Header = attribute.HeaderName;
                        column.Visibility = attribute.VisibleColumn ? Visibility.Visible : Visibility.Hidden;

                        if (!string.IsNullOrWhiteSpace(attribute.SortMemberPath))
                            column.SortMemberPath = attribute.SortMemberPath;

                        if (attribute.SortDirection != null)
                            column.SortDirection = attribute.SortDirection;
                    }
                }
            }
        }
    }
}
