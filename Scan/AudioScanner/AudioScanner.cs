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

            ScanFileEventHandler parsed = FileParsed;
            parsed += AudioScanner_FileParsed;

            return new AudioScan(directory, searchoption, parseAdd, parseUpdate, removeDeadFiles,
                parser, extensionList.ToArray(), existingFiles.ToArray(), ig,
                parsed, ScanDone);
        }

        private void AudioScanner_FileParsed(AudioScan sender, ScanFileEventArgs e)
        {
            switch (e.State)
            {
                case FileState.Added:
                    existingFiles.Add(e.Track);
                    break;
                case FileState.Updated:
                    existingFiles.Remove(e.Path);
                    existingFiles.Add(e.Track);
                    break;
                case FileState.UpdateError:
                    existingFiles.Remove(e.Path);
                    break;
                case FileState.Removed:
                    existingFiles.Remove(e.Path);
                    break;
            }
        }

        public bool IsRunning
        {
            get { return lastScan != null && lastScan.IsRunning; }
        }

        public event ScanCompletedEventHandler ScanDone;
        public event ScanFileEventHandler FileParsed;
    }
}
