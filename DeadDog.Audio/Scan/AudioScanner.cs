using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using DeadDog.Audio.Parsing;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner
    {
        private AudioScan lastScan = null;

        private IMediaParser parser;

        private DirectoryInfo directory;
        private SearchOption searchoption;

        private ExtensionList extensionList;
        private ExistingFilesCollection existingFiles;

        public AudioScan ActiveScan
        {
            get { return lastScan; }
        }

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

        private bool firstscanDone = true;

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

        public AudioScanner(IMediaParser parser, string directory)
            : this(parser, new DirectoryInfo(directory))
        {
        }
        public AudioScanner(IMediaParser parser, DirectoryInfo directory)
            : this(parser, directory, SearchOption.AllDirectories)
        {
        }

        public AudioScanner(IMediaParser parser, string directory, SearchOption searchoption)
            : this(parser, new DirectoryInfo(directory), searchoption)
        {
        }
        public AudioScanner(IMediaParser parser, DirectoryInfo directory, SearchOption searchoption)
            : this(parser, directory, searchoption, ".mp3", ".wma")
        {
        }

        public AudioScanner(IMediaParser parser, string directory, SearchOption searchoption, params string[] extensions)
            : this(parser, new DirectoryInfo(directory), searchoption, extensions)
        {
        }
        public AudioScanner(IMediaParser parser, DirectoryInfo directory, SearchOption searchoption, params string[] extensions)
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

        public AudioScan RunScanner(Library library)
        {
            return RunScanner(library, new string[] { });
        }
        public AudioScan RunScanner(Library library, params string[] ignoreFiles)
        {
            return runScanner(scan => scan.Start(), library, ignoreFiles);
        }

        public AudioScan RunScannerAsync(Library library)
        {
            return RunScannerAsync(library, new string[] { });
        }
        public AudioScan RunScannerAsync(Library library, params string[] ignoreFiles)
        {
            return runScanner(scan => scan.StartAsync(), library, ignoreFiles);
        }

        private AudioScan runScanner(Action<AudioScan> scanStart, Library library, params string[] ignoreFiles)
        {
            if (!directory.Exists)
                throw new ArgumentException("Directory must exist.", "directory");

            if (IsRunning)
                throw new InvalidOperationException("RunScannerAsync is already running.");

            string[] ig = new string[ignoreFiles.Length];
            ignoreFiles.CopyTo(ig, 0);

            ScanFileEventHandler parsed = FileParsed;
            parsed += AudioScanner_FileParsed;

            AudioScan scan = new AudioScan(directory, searchoption, parseAdd, parseUpdate, removeDeadFiles, parser, !firstscanDone, parsed, ScanDone)
            {
                Extensions = extensionList,
                ExistingFiles = existingFiles,
                IgnoredFiles = ig,
                Library = library
            };
            if (!firstscanDone)
                this.existingFiles.Clear();
            firstscanDone = true;

            lastScan = scan;

            scanStart(scan);

            return scan;
        }

        private void AudioScanner_FileParsed(object sender, ScanFileEventArgs e)
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
