using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Scan
{
    public class AudioScan
    {
        private DirectoryInfo directory;
        private SearchOption searchoption;

        private bool parseAdd;
        private bool parseUpdate;
        private bool removeDeadFiles;

        private string[] extensions;
        private RawTrack[] existingFiles;

        private string[] ignoredFiles;

        private event ScanFileEventHandler add, update, error, remove;
        private event ScanCompletedEventHandler done;

        private ScannerState state;
        private ProgressState addProgress;
        private ProgressState updateProgress;
        private ProgressState parseProgress;

        private int removed;

        internal AudioScan(DirectoryInfo directory, SearchOption searchoption,
                bool parseAdd, bool parseUpdate, bool removeDeadFiles,
                string[] extensions, RawTrack[] existingFiles, string[] ignoredFiles,
            ScanFileEventHandler add, ScanFileEventHandler update, ScanFileEventHandler error, ScanFileEventHandler remove,
            ScanCompletedEventHandler done)
        {
            this.directory = directory;
            this.searchoption = searchoption;

            this.parseAdd = parseAdd;
            this.parseUpdate = parseUpdate;
            this.removeDeadFiles = removeDeadFiles;

            this.extensions = extensions;
            this.existingFiles = existingFiles;

            this.ignoredFiles = ignoredFiles;

            this.add = add;
            this.update = update;
            this.error = error;
            this.remove = remove;

            this.state = ScannerState.NotRunning;

            this.addProgress = new ProgressState();
            this.updateProgress = new ProgressState();
            this.parseProgress = new ProgressState();

            this.removed = 0;
        }

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
