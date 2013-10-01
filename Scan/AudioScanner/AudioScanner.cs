using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using DeadDog.Audio.Parsing;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner
    {
        private AudioScan lastScan = null;

        private IDataParser parser;

        private DirectoryInfo directory;
        private SearchOption searchoption;

        private ExtensionList extensionList;
        private ExistingFilesCollection existingFiles;

        public string DirectoryFullName
        {
            get { return directory.FullName; }
        }
        public string DirectoryName
        {
            get { return directory.Name; }
        }
        public bool DirectoryExists
        {
            get { directory.Refresh(); return directory.Exists; }
        }

        public ExtensionList Extensions
        {
            get { return extensionList; }
        }

        public IEnumerable<RawTrack> GetExistingFiles()
        {
            foreach (RawTrack rt in existingFiles)
                yield return rt;
        }

        private bool parseUpdate = true;
        private bool parseAdd = true;
        private bool removeDeadFiles = true;

        public bool ParseUpdate
        {
            get { return parseUpdate; }
            set { parseUpdate = value; }
        }
        public bool ParseAdd
        {
            get { return parseAdd; }
            set { parseAdd = value; }
        }
        public bool RemoveDeadFiles
        {
            get { return removeDeadFiles; }
            set { removeDeadFiles = value; }
        }

        public AudioScanner(IDataParser parser, string directory)
            : this(parser, new DirectoryInfo(directory))
        {
        }
        public AudioScanner(IDataParser parser, DirectoryInfo directory)
            : this(parser, directory, SearchOption.AllDirectories)
        {
        }

        public AudioScanner(IDataParser parser, string directory, SearchOption searchoption)
            : this(parser, new DirectoryInfo(directory), searchoption)
        {
        }
        public AudioScanner(IDataParser parser, DirectoryInfo directory, SearchOption searchoption)
            : this(parser, directory, searchoption, ".mp3", ".wma")
        {
        }

        public AudioScanner(IDataParser parser, string directory, SearchOption searchoption, params string[] extensions)
            : this(parser, new DirectoryInfo(directory), searchoption, extensions)
        {
        }
        public AudioScanner(IDataParser parser, DirectoryInfo directory, SearchOption searchoption, params string[] extensions)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (directory == null)
                throw new ArgumentNullException("directory");

            this.parser = parser;
            this.directory = directory;
            this.searchoption = searchoption;

            extensionList = new ExtensionList(extensions);
            existingFiles = new ExistingFilesCollection();
        }

        public SearchOption SearchOption
        {
            get { return searchoption; }
            set { searchoption = value; }
        }

        public AudioScan RunScannerAsync()
        {
            return RunScannerAsync(new string[] { });
        }
        public AudioScan RunScannerAsync(params string[] ignoreFiles)
        {
            if (!directory.Exists)
                throw new ArgumentException("Directory must exist.", "directory");

            if (IsRunning)
                throw new InvalidOperationException("RunScannerAsync is already running.");

            string[] ig = new string[ignoreFiles.Length];
            ignoreFiles.CopyTo(ig, 0);

            return new AudioScan(directory, searchoption, parseAdd, parseUpdate, removeDeadFiles,
                extensionList.ToArray(), existingFiles.ToArray(), ig,
                FileAdded, FileUpdated, FileError, FileRemoved, ScanDone);
        }

        public bool IsRunning
        {
            get { return lastScan != null && lastScan.IsRunning; }
        }

        public event ScanCompletedEventHandler ScanDone;
        public event ScanFileEventHandler FileAdded;
        public event ScanFileEventHandler FileUpdated;
        public event ScanFileEventHandler FileError;
        public event ScanFileEventHandler FileRemoved;

        #region Path comparison

        private int ComparePath(RawTrack x, RawTrack y)
        {
            return x.FullFilename.CompareTo(y.FullFilename);
        }
        private int ComparePath(FileInfo x, FileInfo y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
        private bool PathEqual(RawTrack x, RawTrack y)
        {
            return x.FullFilename.Equals(y.FullFilename);
        }
        private bool PathEqual(FileInfo x, FileInfo y)
        {
            return x.FullName.Equals(y.FullName);
        }

        private class FileLocater
        {
            private FileInfo file;

            public FileLocater(FileInfo file)
            {
                this.file = file;
            }

            public bool Match(FileInfo file)
            {
                return file.FullName.Equals(this.file.FullName);
            }
        }


        #endregion
    }
}
