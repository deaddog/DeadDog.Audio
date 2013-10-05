using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using DeadDog.Audio.Parsing;

namespace DeadDog.Audio.Scan
{
    public class AudioScan
    {
        private Thread thread;

        private DirectoryInfo directory;
        private SearchOption searchoption;

        private bool parseAdd;
        private bool parseUpdate;
        private bool removeDeadFiles;

        private IDataParser parser;

        private string[] extensions;
        private RawTrack[] existingFiles;

        private string[] ignoredFiles;

        private event ScanFileEventHandler parsed;
        private event ScanCompletedEventHandler done;

        private ScannerState state;
        private ProgressState addProgress;
        private ProgressState updateProgress;
        private ProgressState parseProgress;

        private int removed;

        internal AudioScan(DirectoryInfo directory, SearchOption searchoption,
            bool parseAdd, bool parseUpdate, bool removeDeadFiles, IDataParser parser,
            string[] extensions, RawTrack[] existingFiles, string[] ignoredFiles,
            ScanFileEventHandler parsed,
            ScanCompletedEventHandler done)
        {
            this.thread = new Thread(Run);

            this.directory = directory;
            this.searchoption = searchoption;

            this.parseAdd = parseAdd;
            this.parseUpdate = parseUpdate;
            this.removeDeadFiles = removeDeadFiles;

            this.parser = parser;

            this.extensions = extensions;
            this.existingFiles = existingFiles;

            this.ignoredFiles = ignoredFiles;

            this.parsed = parsed;
            this.done = done;

            this.state = ScannerState.NotRunning;

            this.addProgress = new ProgressState();
            this.updateProgress = new ProgressState();
            this.parseProgress = new ProgressState();

            this.removed = 0;

            state = ScannerState.Scanning;
            thread.Start();
        }

        #region Properties

        public DirectoryInfo Directory
        {
            get { return directory; }
        }
        public SearchOption SearchOption
        {
            get { return searchoption; }
        }

        public bool ParseAdd
        {
            get { return parseAdd; }
        }
        public bool ParseUpdate
        {
            get { return parseUpdate; }
        }
        public bool RemoveDeadFiles
        {
            get { return removeDeadFiles; }
        }

        public string[] FileExtensions
        {
            get { return extensions; }
        }

        public ScannerState State
        {
            get { return state; }
        }
        public ProgressState Added
        {
            get { return addProgress; }
        }
        public ProgressState Updated
        {
            get { return updateProgress; }
        }
        public ProgressState Parsed
        {
            get { return parseProgress; }
        }

        public int Removed
        {
            get { return removed; }
        }

        public bool IsRunning
        {
            get { return state != ScannerState.Completed && state != ScannerState.NotRunning; }
        }

        #endregion

        private void Run()
        {
            List<FileInfo> addFiles = GetFiles();
            List<FileInfo> updateFiles = new List<FileInfo>();
            List<RawTrack> removeFiles = new List<RawTrack>(existingFiles);

            RemoveIgnoredFiles(addFiles, removeFiles);

            if (removeFiles.Count > 0)
                updateFiles.AddRange(FindUpdateFiles(addFiles, removeFiles));

            if (!removeDeadFiles)
                removeFiles.Clear();

            if (removeFiles.Count > 0)
                foreach (RawTrack track in removeFiles)
                    FileParsed(track.FullFilename, track, FileState.Removed);

            //Update scan progress
            removed = removeFiles.Count;
            addProgress.Total = parseAdd ? addFiles.Count : 0;
            updateProgress.Total = parseUpdate ? updateFiles.Count : 0;
            //Update scan state
            state = ScannerState.Parsing;

            if (parseUpdate)
                ParseUpdateFiles(updateFiles);

            if (parseAdd)
                ParseAddFiles(addFiles);

            state = ScannerState.Completed;
            if (done != null)
                done(this, new ScanCompletedEventArgs());
        }

        private void ParseUpdateFiles(List<FileInfo> updateFiles)
        {
            for (int i = 0; i < updateFiles.Count; i++)
            {
                RawTrack rt;
                if (parser.TryParseTrack(updateFiles[i].FullName, out rt))
                {
                    updateProgress.Succes++;
                    int index = existingFiles.BinarySearch(rt, ComparePath);
                    if (index < 0)
                        throw new InvalidOperationException("Updated non-existing file.");

                    if (existingFiles[index].Equals(rt))
                        FileParsed(updateFiles[i].FullName, rt, FileState.Skipped);
                    else
                        FileParsed(updateFiles[i].FullName, rt, FileState.Updated);
                }
                else
                {
                    updateProgress.Error++;
                    FileParsed(updateFiles[i].FullName, null, FileState.UpdateError);
                }
            }
        }
        private void ParseAddFiles(List<FileInfo> addFiles)
        {
            for (int i = 0; i < addFiles.Count; i++)
            {
                RawTrack rt;
                if (parser.TryParseTrack(addFiles[i].FullName, out rt))
                {
                    addProgress.Succes++;
                    FileParsed(addFiles[i].FullName, rt, FileState.Added);
                }
                else
                {
                    addProgress.Error++;
                    FileParsed(addFiles[i].FullName, null, FileState.AddError);
                }
            }
        }

        private IEnumerable<FileInfo> FindUpdateFiles(List<FileInfo> addFiles, List<RawTrack> removeFiles)
        {
            int r = -1; //removeFiles index
            for (int a = 0; a < addFiles.Count; a++)
            {
                int compare = 1;
                while (compare > 0)
                {
                    r++;
                    if (r >= removeFiles.Count)
                        break;
                    compare = ComparePath(addFiles[a], removeFiles[r]);
                }

                if (compare == 0)
                {
                    yield return addFiles[a];
                    addFiles.RemoveAt(a);
                    removeFiles.RemoveAt(r);
                    a--;
                }

                if (r >= removeFiles.Count || removeFiles.Count == 0)
                    break;
            }
        }

        private void RemoveIgnoredFiles(List<FileInfo> addFiles, List<RawTrack> removeFiles)
        {
            for (int i = 0; i < ignoredFiles.Length; i++)
            {
                FileInfo file = new FileInfo(ignoredFiles[i]);
                int a = addFiles.RemoveAll(f => f.FullName.Equals(file.FullName));
                int r = removeFiles.RemoveAll(f => f.FullFilename.Equals(file.FullName));
            }
        }
        private List<FileInfo> GetFiles()
        {
            List<FileInfo> searchFiles = new List<FileInfo>();
            foreach (string ext in extensions)
            {
                FileInfo[] files = directory.GetFiles("*" + ext, searchoption);
                searchFiles.AddRange(files);
            }

            //Windows medtager "minsang.mp3-missing" uden følgende tjek...
            for (int i = 0; i < searchFiles.Count; i++)
            {
                bool ok = false;
                foreach (string ext in extensions)
                    if (searchFiles[i].Extension.ToLower() == ext)
                    {
                        ok = true;
                        break;
                    }
                if (!ok)
                {
                    searchFiles.RemoveAt(i);
                    i--;
                }
            }

            searchFiles.Sort(ComparePath);

            return searchFiles;
        }

        private void FileParsed(string filepath, RawTrack track, FileState state)
        {
            if (parsed != null)
                parsed(this, new ScanFileEventArgs(filepath, track, state));
        }

        #region Path comparison

        private int ComparePath(RawTrack x, RawTrack y)
        {
            return x.FullFilename.CompareTo(y.FullFilename);
        }
        private int ComparePath(FileInfo x, FileInfo y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
        private int ComparePath(RawTrack x, FileInfo y)
        {
            return x.FullFilename.CompareTo(y.FullName);
        }
        private int ComparePath(FileInfo x, RawTrack y)
        {
            return x.FullName.CompareTo(y.FullFilename);
        }

        private bool PathEqual(RawTrack x, RawTrack y)
        {
            return x.FullFilename.Equals(y.FullFilename);
        }
        private bool PathEqual(FileInfo x, FileInfo y)
        {
            return x.FullName.Equals(y.FullName);
        }
        private bool PathEqual(RawTrack x, FileInfo y)
        {
            return x.FullFilename.Equals(y.FullName);
        }
        private bool PathEqual(FileInfo x, RawTrack y)
        {
            return x.FullName.Equals(y.FullFilename);
        }

        #endregion

        public struct ProgressState
        {
            private int total;
            private int succes;
            private int error;

            public int Total
            {
                get { return total; }
                set { total = value; }
            }
            public int Succes
            {
                get { return succes; }
                set { succes = value; }
            }
            public int Error
            {
                get { return error; }
                set { error = value; }
            }
            public int Progress
            {
                get { return succes + error; }
            }
        }
    }
}
