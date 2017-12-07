using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public ICollection<RowFilter> _dgListFilter = new List<RowFilter>();

        #region Window

        public Settings()
        {
            InitializeComponent();
        }

        private void SettingsApplication_Loaded(object sender, RoutedEventArgs e)
        {
            ReadSettings();
        }

        private void SettingsApplication_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Main menu

        private void MiSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void MiClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Element methods

        private void CboxAutoload_Click(object sender, RoutedEventArgs e)
        {
            SetValueSettings(autoload: cboxAutoload.IsChecked.Value);
        }

        private void CboxAutoupdate_Click(object sender, RoutedEventArgs e)
        {
            SetValueSettings(autoupdateVersion: cboxAutoload.IsChecked.Value);
        }

        private void BtnRestoreFilter_Click(object sender, RoutedEventArgs e)
        {
            RestoreFilterByDefault();
        }

        #region Element DirectoryCache

        private void TxtDirectoryCache_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueSettings(txtDirectoryCache.Text);
        }

        private void BtnSelectDirectoryCache_Click(object sender, RoutedEventArgs e)
        {
            var defaultSettings = Properties.Settings.Default;
            string selectedDirectory = Dialog.SelectDirectoryCache(defaultSettings.DirectoryCache);
            if (!String.IsNullOrWhiteSpace(selectedDirectory))
            {
                SetValueSettings(selectedDirectory);
                ReadSettings();
            }
        }

        private void BtnOpenDirectoryCache_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtDirectoryCache.Text))
                Dialog.OpenDirectory(txtDirectoryCache.Text);
        }

        #endregion

        #endregion

        private void SaveSettings()
        {
            var defaultSettings = Properties.Settings.Default;
            defaultSettings.DirectoryCache = txtDirectoryCache.Text;
            defaultSettings.Autoload = cboxAutoload.IsChecked.Value;
            defaultSettings.AutoupdateVersion = cboxAutoupdate.IsChecked.Value;
            defaultSettings.ListFilterFiles = GetListInDgFilter();
        }

        private void ReadSettings()
        {
            var defaultSettings = Properties.Settings.Default;
            txtDirectoryCache.Text = defaultSettings.DirectoryCache;
            cboxAutoload.IsChecked = defaultSettings.Autoload;
            cboxAutoupdate.IsChecked = defaultSettings.AutoupdateVersion;

            try
            {
                var defaultProperties = Properties.Settings.Default;
                if (defaultProperties.ListFilterFiles == null)
                    FillDgFilter(defaultProperties.ListFilterFilesPredefined);
                else
                    FillDgFilter(defaultProperties.ListFilterFiles);
            }
            catch (Exception)
            {
                Dialog.ShowMessage("Ошибка загрузки настроек фильтра.");
            }
        }

        private void SetValueSettings(string directoryCache = null, bool? autoload = null, bool? autoupdateVersion = null)
        {
            var defaultSettings = Properties.Settings.Default;
            if (directoryCache != null)
                defaultSettings.DirectoryCache = directoryCache;
            if (autoload != null)
                defaultSettings.Autoload = (bool)autoload;
            if (autoupdateVersion != null)
                defaultSettings.AutoupdateVersion = (bool)autoupdateVersion;
        }

        #region Filters

        private void RestoreFilterByDefault()
        {
            FillDgFilter(Properties.Settings.Default.ListFilterFilesPredefined);
        }

        private void FillDgFilter(StringCollection collection)
        {
            _dgListFilter.Clear();
            foreach (string itemFilter in collection)
            {
                string[] arrayFilter = itemFilter.Split('|');
                _dgListFilter.Add(new RowFilter(arrayFilter[0], arrayFilter[1]));
            }
            SetItemSourceDgFilter();
        }

        private void SetItemSourceDgFilter()
        {
            dgFilter.ItemsSource = null;
            dgFilter.ItemsSource = _dgListFilter;
        }

        private StringCollection GetListInDgFilter()
        {
            StringCollection collection = new StringCollection();

            StringBuilder sb = new StringBuilder();
            foreach (RowFilter itemFilter in _dgListFilter)
            {
                sb.Clear();
                sb.Append(itemFilter.Present);
                sb.Append('|');
                sb.Append(itemFilter.Filter);
                collection.Add(sb.ToString());
            }

            return collection;
        }

        #endregion

        private void DgFilter_AutoGeneratedColumns(object sender, EventArgs e)
        {
            foreach (PropertyInfo propInfo in new RowFilter().GetType().GetProperties())
            {
                DataGridColumn column = dgFilter.Columns.FirstOrDefault(f => (string)f.Header == propInfo.Name);
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
