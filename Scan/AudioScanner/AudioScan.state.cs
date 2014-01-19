using System;
using System.IO;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScan
    {
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
