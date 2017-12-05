using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using wf = System.Windows.Forms;
using System.Diagnostics;

namespace ControlModifiedFiles
{
    internal static class Dialog
    {
        internal static bool DialogQuestion(string textQuestion)
        {
            return MessageBox.Show(textQuestion, "Вопрос", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        internal static void ShowMessage(string textMessage)
        {
            MessageBox.Show(textMessage);
        }
        
        internal static string SelectDirectoryCache(string currentDirectory)
        {
            string selectedDirectory = String.Empty;

            var dialog = new wf.FolderBrowserDialog()
            {
                Description = "Каталог сохранения версий файлов",
                //RootFolder = Environment.SpecialFolder.Personal,
                SelectedPath = currentDirectory,
                ShowNewFolderButton = true
            };
            var result = dialog.ShowDialog();

            if (result == wf.DialogResult.OK)
                selectedDirectory = dialog.SelectedPath;

            return selectedDirectory;
        }
        internal static void OpenDirectory(string path)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\""));
        }
    }
}
