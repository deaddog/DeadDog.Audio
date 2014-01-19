using System;
using System.IO;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScan
    {
        private DirectoryInfo directory;
        private SearchOption searchoption;
        public DirectoryInfo Directory
        {
            get { return directory; }
        }
        public SearchOption SearchOption
        {
            get { return searchoption; }
        }

        private bool parseAdd;
        private bool parseUpdate;
        private bool removeDeadFiles;
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

        private string[] extensions;
        public string[] FileExtensions
        {
            get { return extensions; }
        }

        private ScannerState state;
        public ScannerState State
        {
            get { return state; }
        }

        private int added;
        private int updated;
        private int skipped;
        private int error;
        private int removed;
        private int total;
        public int Added
        {
            get { return added; }
        }
        public int Updated
        {
            get { return updated; }
        }
        public int Skipped
        {
            get { return skipped; }
        }
        public int Errors
        {
            get { return error; }
        }
        public int Removed
        {
            get { return removed; }
        }
        public int Total
        {
            get { return total; }
        }

        public bool IsRunning
        {
            get { return state != ScannerState.Completed && state != ScannerState.NotRunning; }
        }
    }
}
