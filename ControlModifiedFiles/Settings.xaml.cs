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
using System.Windows.Shapes;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
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
            var defaultSettings = Properties.Settings.Default;
            defaultSettings.DirectoryCache = txtDirectoryCache.Text;
            defaultSettings.Autoload = cboxAutoload.IsChecked.Value;
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

        #region Element DirectoryCache

        private void TxtDirectoryCache_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueSettings(txtDirectoryCache.Text);
        }

        private void SetValueSettings(string directoryCache = null, bool? autoload = null)
        {
            var defaultSettings = Properties.Settings.Default;
            if (directoryCache != null)
                defaultSettings.DirectoryCache = directoryCache;
            if (autoload != null)
                defaultSettings.Autoload = (bool)autoload;
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

        private void ReadSettings()
        {
            var defaultSettings = Properties.Settings.Default;
            txtDirectoryCache.Text = defaultSettings.DirectoryCache;
            cboxAutoload.IsChecked = defaultSettings.Autoload;
        }

    }
}
