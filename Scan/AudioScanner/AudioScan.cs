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

        private bool isrunning = false;
        public bool IsRunning
        {
            get { return isrunning; }
        }

        #endregion

        private void Run()
        {
            isrunning = true;

            state = ScannerState.Scanning;

            List<FileInfo> addFiles = GetFiles();
            List<FileInfo> updateFiles = new List<FileInfo>();
            List<RawTrack> removeFiles = new List<RawTrack>(existingFiles);

            RemoveIgnoredFiles(addFiles, removeFiles);

            addFiles.Sort(ComparePath);

            if (removeFiles.Count > 0)
            {
                int t = 0; //removeFiles index
                for (int i = 0; i < addFiles.Count; i++)
                {
                    int comp = ComparePath(addFiles[i], removeFiles[t]);
                    while (comp > 0)
                    {
                        t++;
                        if (t >= removeFiles.Count)
                            break;
                        comp = ComparePath(addFiles[i], removeFiles[t]);
                    }

                    if (comp == 0)
                    {
                        updateFiles.Add(addFiles[i]);
                        addFiles.RemoveAt(i);
                        removeFiles.RemoveAt(t);
                        i--;
                    }

                    if (t >= removeFiles.Count)
                        break;

                    if (removeFiles.Count == 0)
                        break;
                }
            }

            if (!removeDeadFiles)
                removeFiles.Clear();

            if (removeFiles.Count > 0)
                foreach (RawTrack track in removeFiles)
                    FileParsed(track.FullFilename, track, FileState.Removed);

            //Scanning complete, setting values of tracer.Progress
            //Setting how many files were removed from existingFiles (if any)
            removed = removeFiles.Count;

            addProgress.Total = parseAdd ? addFiles.Count : 0;
            updateProgress.Total = parseUpdate ? updateFiles.Count : 0;

            //Change scanner state and report progress
            state = ScannerState.Parsing;

            if (parseUpdate)
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

            if (parseAdd)
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

            state = ScannerState.Completed;

            isrunning = true;
            if (done != null)
                done(this, new ScanCompletedEventArgs());
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
