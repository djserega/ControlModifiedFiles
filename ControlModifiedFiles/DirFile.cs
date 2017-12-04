using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlModifiedFiles
{
    internal class DirFile
    {
        internal string Path { get; }

        #region constructors

        public DirFile()
        {
        }

        public DirFile(string path)
        {
            Path = path;
        }

        #endregion

        #region internal methods

        internal ulong GetDirSize() => CalculateSize(Path);

        internal ulong GetFileSize() => CalculateFileSize(Path);

        internal Tuple<DateTime, DateTime> GetDateCreateEdited() => DateCreateEdited(Path);

        internal string GetSizeFormat(ulong size)
        {

            if (size < 1024)
                return (size).ToString("F0") + " bytes";

            else if ((size >> 10) < 1024)
                return (size / (float)1024).ToString("F1") + " KB";

            else if ((size >> 20) < 1024)
                return ((size >> 10) / (float)1024).ToString("F1") + " MB";

            else if ((size >> 30) < 1024)
                return ((size >> 20) / (float)1024).ToString("F1") + " GB";

            else if ((size >> 40) < 1024)
                return ((size >> 30) / (float)1024).ToString("F1") + " TB";

            else if ((size >> 50) < 1024)
                return ((size >> 40) / (float)1024).ToString("F1") + " PB";

            else
                return ((size >> 50) / (float)1024).ToString("F0") + " EB";

        }

        internal void OpenDirectory()
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{Path}\""));
        }

        internal DateTime CompareDatePlus(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) == 1)
                date1 = date2;

            return date1;
        }

        internal DateTime CompareDateMinus(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) == -1)
                date1 = date2;
            return date1;
        }

        internal string GetFileChecked(Window owner)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "Внешняя обработка (*.epf)|*.epf" +
                "|Внешний отчет (*.erf)|*.erf" +
                "|Поддерживаемые файлы|*.epf,*.erf",
                Multiselect = false,
                Title = "Выбор файла контроля"
            };

            bool? result = openFile.ShowDialog(owner);

            if (result.HasValue && result.Value)
                return openFile.FileName;
            else
                return string.Empty;
        }

        internal void SaveFile(string path, string data)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.Write(data);
            }
        }
        internal string LoadFile(string path)
        {
            string data;
            using (StreamReader streamReader = new StreamReader(path))
            {
                data = streamReader.ReadToEnd();
            }
            return data;
        }

        #endregion

        #region private methods

        private void DeleteDir(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                DeleteSubDir(path);
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }

        private void DeleteSubDir(string path)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            foreach (System.IO.FileInfo file in dirInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }
            foreach (DirectoryInfo currentSubDir in dirInfo.GetDirectories())
            {
                try
                {
                    DeleteSubDir(currentSubDir.FullName);
                    currentSubDir.Delete(true);
                }
                catch (Exception)
                {
                }
            }

        }

        private ulong CalculateFileSize(string path)
        {
            return (ulong)new System.IO.FileInfo(path).Length;
        }

        private ulong CalculateSize(string path)
        {
            ulong size = 0;

            foreach (string files in Directory.GetFiles(path))
                size += (ulong)new System.IO.FileInfo(files).Length;

            foreach (string dir in Directory.GetDirectories(path))
                size += CalculateSize(dir);

            return size;
        }

        private Tuple<DateTime, DateTime> DateCreateEdited(string path)
        {

            DateTime dateCreate = DateTime.MaxValue;
            DateTime dateEdit = DateTime.MinValue;

            foreach (string files in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(files);
                dateCreate = CompareDatePlus(dateCreate, fileInfo.CreationTime);
                dateEdit = CompareDateMinus(dateEdit, fileInfo.LastWriteTime);
            }


            return new Tuple<DateTime, DateTime>(dateCreate, dateEdit);

        }

        #endregion
    }
}
