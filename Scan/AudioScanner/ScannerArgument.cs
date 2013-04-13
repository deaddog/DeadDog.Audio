using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner
    {
        private class ScannerArgument
        {
            private DirectoryInfo directory;
            private SearchOption searchoption;

            private bool parseAdd;
            private bool parseUpdate;
            private bool removeDeadFiles;

            private string[] extensions;
            private RawTrack[] existingFiles;

            private string[] ignoredFiles;

            private ScannerTracer tracer;

            public ScannerArgument(DirectoryInfo directory, SearchOption searchoption,
                bool parseAdd, bool parseUpdate, bool removeDeadFiles,
                string[] extensions, RawTrack[] existingFiles)
                : this(directory, searchoption, parseAdd, parseUpdate, removeDeadFiles,
                       extensions, existingFiles, new string[0])
            {
            }

            public ScannerArgument(DirectoryInfo directory, SearchOption searchoption,
                bool parseAdd, bool parseUpdate, bool removeDeadFiles,
                string[] extensions, RawTrack[] existingFiles, string[] ignoredFiles)
            {
                this.directory = directory;
                this.searchoption = searchoption;

                this.parseAdd = parseAdd;
                this.parseUpdate = parseUpdate;
                this.removeDeadFiles = removeDeadFiles;

                this.extensions = extensions;
                this.existingFiles = existingFiles;

                this.ignoredFiles = ignoredFiles;

                this.tracer = new ScannerTracer();
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
            public RawTrack[] ExistingFiles
            {
                get { return existingFiles; }
            }

            public string[] IgnoredFiles
            {
                get { return ignoredFiles; }
            }

            public ScannerTracer Tracer
            {
                get { return tracer; }
            }
        }
    }
}
