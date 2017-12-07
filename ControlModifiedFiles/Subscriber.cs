using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ControlModifiedFiles
{
    internal class Subscriber
    {
        #region Properties

        internal Dictionary<FileSubscriber, FileSystemWatcher> DictionaryWatcher { get; private set; }

        private string _prefixNameVersion = "{version ";
        private string _fileNameWithoutExtension;
        private string _fileNameWithVersion;

        #endregion

        #region Constructors

        internal Subscriber()
        {
            DictionaryWatcher = new Dictionary<FileSubscriber, FileSystemWatcher>();
        }

        #endregion

        #region Internal methods

        internal void SubscribeChangeFile(FileSubscriber file)
        {
            Subscribe(file);
        }

        internal void SubscribeChangeFiles(List<FileSubscriber> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Subscribe(list[i]);
            }
        }

        internal void UnsubscribeChangeFile(FileSubscriber file)
        {
            try
            {
                var keyWatcher = DictionaryWatcher.FirstOrDefault(f => f.Key == file);
                if (keyWatcher.Key != null)
                {
                    FileSystemWatcher watcher = keyWatcher.Value;
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    DictionaryWatcher.Remove(keyWatcher.Key);
                }
            }
            catch (Exception)
            {
            }
        }

        internal Task<List<FileSubscriber>> LoadVersionFilesAsync(List<FileSubscriber> asyncList)
        {
            return Task.Run(() => LoadVersionFiles(asyncList));
        }
        internal List<FileSubscriber> LoadVersionFiles(List<FileSubscriber> asyncList)
        {
            List<FileSubscriber> asyncListResult = new List<FileSubscriber>();

            foreach (FileSubscriber file in asyncList)
            {
                FileInfo fileInfo = new FileInfo(file.Path);

                if (fileInfo.Exists)
                    file.Version = GetCurrentVersionFile(fileInfo, file, false);
                else
                {
                    Dialog.ShowMessage($"Файл '{file.Path}' перемещен или удален.");
                    file.Checked = false;
                    UnsubscribeChangeFile(file);
                }
                asyncListResult.Add(file);
            }

            return asyncListResult;
        }

        #endregion

        #region Private methods

        private void GetFileNameWithoutExtension(FileInfo fileInfo)
        {
            long startIndex = fileInfo.Name.Length - fileInfo.Extension.Length;
            _fileNameWithoutExtension = fileInfo.Name.Remove((int)startIndex);
        }

        private void GetFileNameWithVersion()
        {
            _fileNameWithVersion = $"{_fileNameWithoutExtension} {_prefixNameVersion}";
        }

        #endregion

        #region Subscribe

        private void Subscribe(FileSubscriber file)
        {
            if (file.Checked
                && !String.IsNullOrWhiteSpace(file.Path))
            {
                EnableSubscription(file);
            }
        }

        private void EnableSubscription(FileSubscriber file)
        {
            FileInfo fileInfo = new FileInfo(file.Path);

            file.DirectoryVersion = GetDirectoryVersion(fileInfo);

            FileSystemWatcher watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            watcher.Changed += new FileSystemEventHandler(ChangedFile);
            watcher.EnableRaisingEvents = true;

            DictionaryWatcher.Add(file, watcher);
        }

        private void ChangedFile(object sender, FileSystemEventArgs e)
        {
            try
            {
                var keyWatcher = DictionaryWatcher.First(f => f.Key.Path == e.FullPath);
                FileSubscriber file = keyWatcher.Key;

                FileInfo fileInfo = new FileInfo(file.Path);
                string newFileName = GetFileNameVersion(fileInfo, file);

                if (!String.IsNullOrWhiteSpace(newFileName))
                    fileInfo.CopyTo(newFileName);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Version

        private string GetFileNameVersion(FileInfo fileInfo, FileSubscriber file)
        {
            int version = GetNewVersion(fileInfo, file, fileInfo.Extension);

            if (version == 0)
                return null;

            string fileNameVersion =
                $"{file.DirectoryVersion}" +
                $"\\{_fileNameWithoutExtension} " +
                $"{_prefixNameVersion}" +
                $"{version}}}" +
                $"{fileInfo.Extension}";

            return fileNameVersion;
        }

        private string GetDirectoryVersion(FileInfo fileInfo)
        {
            string defaultDirectoryCache = Properties.Settings.Default.DirectoryCache;

            if (String.IsNullOrWhiteSpace(defaultDirectoryCache))
            {
                DirectoryInfo directoryInfoVersion = new DirectoryInfo($"{fileInfo.Directory}\\_Version\\");
                if (!directoryInfoVersion.Exists)
                    directoryInfoVersion.Create();

                GetFileNameWithoutExtension(fileInfo);

                DirectoryInfo directoryInfoFile = new DirectoryInfo(
                    $"{directoryInfoVersion.FullName}\\{_fileNameWithoutExtension}");
                if (!directoryInfoFile.Exists)
                    directoryInfoFile.Create();

                return directoryInfoFile.FullName;
            }
            else
            {
                return defaultDirectoryCache + "\\";
            }
        }

        private int GetNewVersion(FileInfo fileInfo, FileSubscriber file, string fileExtension)
        {
            int newVersion = GetCurrentVersionFile(fileInfo, file);

            newVersion++;

            return newVersion;
        }

        private int GetCurrentVersionFile(FileInfo fileInfo, FileSubscriber file, bool controlCurrentHash = true)
        {
            if (String.IsNullOrWhiteSpace(file.DirectoryVersion))
                return 0;

            GetFileNameWithoutExtension(fileInfo);
            GetFileNameWithVersion();

            FileInfo fileInfoMaxEdited = GetFileLastVersion(file, fileInfo.Extension, controlCurrentHash);

            return GetNumberVersionIsFileName(fileInfoMaxEdited, fileInfo);
        }

        private FileInfo GetFileLastVersion(FileSubscriber file, string fileExtension, bool controlCurrentHash = false)
        {
            FileInfo fileInfoMaxEdited = null;

            string currentHash = GetMD5(file.Path);
            if (String.IsNullOrWhiteSpace(currentHash))
            {
                file.Checked = false;
                UnsubscribeChangeFile(file);
                return fileInfoMaxEdited;
            }

            FileInfo[] filesVersions = new DirectoryInfo(file.DirectoryVersion).GetFiles($"{_fileNameWithVersion}*{fileExtension}");

            DateTime dateTimeMaxEdited = DateTime.MinValue;
            foreach (FileInfo versionFile in filesVersions)
            {
                string md5VersionFile = GetMD5(versionFile.FullName);
                if (!String.IsNullOrWhiteSpace(md5VersionFile))
                {
                    if (currentHash == md5VersionFile && controlCurrentHash)
                        return null;

                    if (dateTimeMaxEdited <= versionFile.LastWriteTime)
                    {
                        fileInfoMaxEdited = versionFile;
                        dateTimeMaxEdited = versionFile.LastWriteTime;
                    };
                }
            };

            return fileInfoMaxEdited;
        }

        private int GetNumberVersionIsFileName(FileInfo fileInfoMaxEdited, FileInfo fileInfo)
        {
            int currentVersion = 0;

            if (fileInfoMaxEdited != null)
            {
                if (fileInfo.Equals(fileInfoMaxEdited))
                    return 0;

                string nameFileVersion = fileInfoMaxEdited.Name;
                string stringVersion = nameFileVersion.Remove(0, _fileNameWithVersion.Length);
                int startIndex = stringVersion.Length - ($"}}{fileInfo.Extension}".Length);
                stringVersion = stringVersion.Remove(startIndex);
                int.TryParse(stringVersion, out currentVersion);
            }

            return currentVersion;
        }

        private string GetMD5(string path)
        {
            string hash = "";

            return DateTime.Now.Ticks.ToString();

            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    //using (FileStream stream = File.OpenRead(path))
                    {
                        byte[] hashByte = md5.ComputeHash(stream);
                        hash = BitConverter.ToString(hashByte).Replace("-", "").ToLowerInvariant();
                    }
                }
                return hash;
            }
            catch (FileNotFoundException)
            {
                Dialog.ShowMessage($"Файл '{path}' перемещен или удален.");
                return hash;
            }
        }

        #endregion
    }
}
