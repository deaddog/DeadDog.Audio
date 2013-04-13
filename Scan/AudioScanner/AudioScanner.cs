using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner : IDisposable
    {
        private BackgroundWorker worker;
        private ADataParser parser;

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

        public AudioScanner(ADataParser parser, string directory)
            : this(parser, new DirectoryInfo(directory))
        {
        }
        public AudioScanner(ADataParser parser, DirectoryInfo directory)
            : this(parser, directory, SearchOption.AllDirectories)
        {
        }

        public AudioScanner(ADataParser parser, string directory, SearchOption searchoption)
            : this(parser, new DirectoryInfo(directory), searchoption)
        {
        }
        public AudioScanner(ADataParser parser, DirectoryInfo directory, SearchOption searchoption)
            : this(parser, directory, searchoption, ".mp3", ".wma")
        {
        }

        public AudioScanner(ADataParser parser, string directory, SearchOption searchoption, params string[] extensions)
            : this(parser, new DirectoryInfo(directory), searchoption, extensions)
        {
        }
        public AudioScanner(ADataParser parser, DirectoryInfo directory, SearchOption searchoption, params string[] extensions)
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

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ScannerArgument arg = e.Argument as ScannerArgument;
            ScannerTracer tracer = arg.Tracer;
            tracer.Progress.Existing = arg.ExistingFiles.Length;
            tracer.State = ScannerState.Scanning;
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, new ScanProgressChangedEventArgs(tracer));

            List<FileInfo> addFiles = GetFilesFromDir(arg);
            List<FileInfo> updateFiles = new List<FileInfo>();
            List<FileInfo> removeFiles = new List<FileInfo>(arg.ExistingFiles.Length);
            foreach (RawTrack rt in arg.ExistingFiles)
                removeFiles.Add(new FileInfo(rt.FullFilename));

            if (arg.IgnoredFiles.Length > 0)
            {
                for (int i = 0; i < arg.IgnoredFiles.Length; i++)
                {
                    FileInfo file = new FileInfo(arg.IgnoredFiles[i]);
                    FileLocater fl = new FileLocater(file);
                    int a = addFiles.RemoveAll(fl.Match);
                    int r = removeFiles.RemoveAll(fl.Match);
                }
            }

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

            if (!arg.RemoveDeadFiles)
                removeFiles.Clear();

            if (removeFiles.Count > 0)
                foreach (FileInfo file in removeFiles)
                    existingFiles.Remove(file.FullName);

            //Scanning complete, setting values of tracer.Progress
            //Setting how many files were removed from existingFiles (if any)
            tracer.Progress.Removed = removeFiles.Count;

            if (arg.ParseAdd)
                tracer.Progress.AddTotal = addFiles.Count;
            else
                tracer.Progress.AddTotal = 0;

            if (arg.ParseUpdate)
                tracer.Progress.UpdateTotal = updateFiles.Count;
            else
                tracer.Progress.UpdateTotal = 0;

            //Change scanner state and report progress
            tracer.State = ScannerState.Parsing;
            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, new ScanProgressChangedEventArgs(tracer));

            List<FileInfo> errorFiles = new List<FileInfo>();
            List<RawTrack> addData = new List<RawTrack>(addFiles.Count);
            List<RawTrack> updateData = new List<RawTrack>(updateFiles.Count);

            if (arg.ParseUpdate)
                for (int i = 0; i < updateFiles.Count; i++)
                {
                    RawTrack rt;
                    if (parser.TryParseTrack(updateFiles[i].FullName, out rt))
                    {
                        updateData.Add(rt);
                        tracer.Progress.Updated++;
                    }
                    else
                    {
                        errorFiles.Add(updateFiles[i]);
                        tracer.Progress.UpdateError++;
                    }
                    if (worker.WorkerReportsProgress)
                        worker.ReportProgress(0, new ScanProgressChangedEventArgs(tracer));
                }

            if (arg.ParseAdd)
                for (int i = 0; i < addFiles.Count; i++)
                {
                    RawTrack rt;
                    if (parser.TryParseTrack(addFiles[i].FullName, out rt))
                    {
                        addData.Add(rt);
                        existingFiles.Add(rt);
                        tracer.Progress.Added++;
                    }
                    else
                    {
                        errorFiles.Add(addFiles[i]);
                        tracer.Progress.AddError++;
                    }
                    if (worker.WorkerReportsProgress)
                        worker.ReportProgress(0, new ScanProgressChangedEventArgs(tracer));
                }

            e.Result = new ScanCompletedEventArgs(removeFiles, errorFiles, addData, updateData, arg.ExistingFiles);

            tracer.State = ScannerState.Completed;

            if (worker.WorkerReportsProgress)
                worker.ReportProgress(0, new ScanProgressChangedEventArgs(tracer));

            isrunning = false;
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e.UserState as ScanProgressChangedEventArgs);
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RunScannerCompleted != null)
                RunScannerCompleted(this, e.Result as ScanCompletedEventArgs);
        }

        public bool ScannerReportsProgress
        {
            get { return worker.WorkerReportsProgress; }
            set { worker.WorkerReportsProgress = value; }
        }
        public SearchOption SearchOption
        {
            get { return searchoption; }
            set { searchoption = value; }
        }

        public ScannerTracer RunScannerAsync()
        {
            if (!directory.Exists)
                throw new ArgumentException("Directory must exist.", "directory");

            if (IsRunning)
                throw new InvalidOperationException("RunScannerAsync is already running.");

            isrunning = true;

            ScannerArgument arg = new ScannerArgument(directory, searchoption,
                parseAdd, parseUpdate, removeDeadFiles,
                extensionList.ToArray(), existingFiles.ToArray());

            worker.RunWorkerAsync(arg);
            return arg.Tracer;
        }
        public ScannerTracer RunScannerAsync(params string[] ignoreFiles)
        {
            if (!directory.Exists)
                throw new ArgumentException("Directory must exist.", "directory");

            if (IsRunning)
                throw new InvalidOperationException("RunScannerAsync is already running.");

            isrunning = true;

            ScannerArgument arg = new ScannerArgument(directory, searchoption,
                parseAdd, parseUpdate, removeDeadFiles,
                extensionList.ToArray(), existingFiles.ToArray(), ignoreFiles);

            worker.RunWorkerAsync(arg);
            return arg.Tracer;
        }

        private bool isrunning = false;
        public bool IsRunning
        {
            get { return isrunning || worker.IsBusy; }
        }

        private List<FileInfo> GetFilesFromDir(ScannerArgument argument)
        {
            List<FileInfo> searchFiles = new List<FileInfo>();
            foreach (string ext in argument.FileExtensions)
            {
                FileInfo[] files = argument.Directory.GetFiles("*" + ext, argument.SearchOption);
                searchFiles.AddRange(files);
            }

            //Windows medtager "minsang.mp3-missing" uden følgende tjek...
            for (int i = 0; i < searchFiles.Count; i++)
            {
                bool ok = false;
                foreach (string ext in argument.FileExtensions)
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

        public event ScanCompletedEventHandler RunScannerCompleted;
        public event ScanProgressChangedEventHandler ProgressChanged;

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

        public void Dispose()
        {
            worker.Dispose();
        }
    }
}
