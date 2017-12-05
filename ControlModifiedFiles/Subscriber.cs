using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Subscriber
    {
        #region Properties

        internal Dictionary<FileSubscriber, FileSystemWatcher> DictionaryWatcher { get; private set; }

        private string _prefixNameVersion = "{version ";



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
                var keyWatcher = DictionaryWatcher.First(f => f.Key == file);
                FileSystemWatcher watcher = keyWatcher.Value;
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                DictionaryWatcher.Remove(keyWatcher.Key);
            }
            catch (Exception)
            {
            }
        }

        internal void LoadVersionFiles(List<FileSubscriber> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                FileInfo fileInfo = new FileInfo(list[i].Path);
                string fileNameWithoutExtension = GetFileNameWithoutExtension(fileInfo);
                int version = GetCurrentVersionFile(fileInfo, list[i], fileNameWithoutExtension, fileInfo.Extension, false);
                list[i].Version = version;
            }
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

            FileSystemWatcher watcher = new FileSystemWatcher(fileInfo.DirectoryName)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = fileInfo.Name
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

        private string GetFileNameWithoutExtension(FileInfo fileInfo)
        {
            long startIndex = fileInfo.Name.Length - fileInfo.Extension.Length;
            return fileInfo.Name.Remove((int)startIndex);
        }

        #endregion

        #region Version

        private string GetFileNameVersion(FileInfo fileInfo, FileSubscriber file)
        {
            string fileNameWithoutExtension = GetFileNameWithoutExtension(fileInfo);
            int version = GetNewVersion(fileInfo, file, fileNameWithoutExtension, fileInfo.Extension);

            if (version == 0)
                return null;

            string fileNameVersion =
                $"{file.DirectoryVersion}" +
                $"\\{fileNameWithoutExtension} " +
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

                DirectoryInfo directoryInfoFile = new DirectoryInfo(
                    $"{directoryInfoVersion.FullName}\\{GetFileNameWithoutExtension(fileInfo)}");
                if (!directoryInfoFile.Exists)
                    directoryInfoFile.Create();

                return directoryInfoFile.FullName;
            }
            else
            {
                return defaultDirectoryCache + "\\";
            }
        }

        private int GetNewVersion(FileInfo fileInfo, FileSubscriber file, string fileNameWithoutExtension, string fileExtension)
        {
            int newVersion = GetCurrentVersionFile(fileInfo, file, fileNameWithoutExtension, fileExtension);

            newVersion++;

            return newVersion;
        }

        private int GetCurrentVersionFile(FileInfo fileInfo, FileSubscriber file, string fileNameWithoutExtension,
            string fileExtension, bool controlCurrentHash = true)
        {
            if (String.IsNullOrWhiteSpace(file.DirectoryVersion))
                return 0;

            string fileNameWithVersion = $"{fileNameWithoutExtension} {_prefixNameVersion}";

            FileInfo fileInfoMaxEdited = GetFileLastVersion(file, fileNameWithVersion, fileExtension, controlCurrentHash);

            return GetNumberVersionIsFileName(fileInfoMaxEdited, fileInfo, fileNameWithVersion, fileExtension);
        }

        private FileInfo GetFileLastVersion(FileSubscriber file, string fileNameWithVersion, string fileExtension, bool controlCurrentHash = false)
        {
            FileInfo fileInfoMaxEdited = null;

            string currentHash = GetMD5(file.Path);

            FileInfo[] filesVersions = new DirectoryInfo(file.DirectoryVersion).GetFiles($"{fileNameWithVersion}*{fileExtension}");

            DateTime dateTimeMaxEdited = DateTime.MinValue;
            foreach (FileInfo versionFile in filesVersions)
            {
                if (currentHash == GetMD5(versionFile.FullName)
                    && controlCurrentHash)
                    return null;

                if (dateTimeMaxEdited <= versionFile.LastWriteTime)
                {
                    fileInfoMaxEdited = versionFile;
                    dateTimeMaxEdited = versionFile.LastWriteTime;
                };
            };

            return fileInfoMaxEdited;
        }

        private int GetNumberVersionIsFileName(FileInfo fileInfoMaxEdited, FileInfo fileInfo, string fileNameWithVersion, string fileExtension)
        {
            int currentVersion = 0;

            if (fileInfoMaxEdited != null)
            {
                if (fileInfo.Equals(fileInfoMaxEdited))
                    return 0;

                string nameFileVersion = fileInfoMaxEdited.Name;
                string stringVersion = nameFileVersion.Remove(0, fileNameWithVersion.Length);
                int startIndex = stringVersion.Length - ($"}}{fileExtension}".Length);
                stringVersion = stringVersion.Remove(startIndex);
                int.TryParse(stringVersion, out currentVersion);
            }

            return currentVersion;
        }

        private string GetMD5(string path)
        {
            string hash;
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    byte[] hashByte = md5.ComputeHash(stream);
                    hash = BitConverter.ToString(hashByte).Replace("-", "").ToLowerInvariant();
                }
            }
            return hash;
        }

        #endregion
    }
}
