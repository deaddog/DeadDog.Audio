using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using DeadDog.Audio.Parsing;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScan
    {
        private Thread thread;

        private DirectoryInfo directory;
        private SearchOption searchoption;

        private bool parseAdd;
        private bool parseUpdate;
        private bool removeDeadFiles;

        private IDataParser parser;

        private string[] extensions;
        private Dictionary<FileInfo, RawTrack> existingFiles;

        private FileInfo[] ignoredFiles;

        private event ScanFileEventHandler parsed;
        private event ScanCompletedEventHandler done;

        private ScannerState state;

        private int added;
        private int updated;
        private int skipped;
        private int error;
        private int removed;
        private int total;

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
            this.existingFiles = existingFiles.ToDictionary(rt => rt.File);

            this.ignoredFiles = (from s in ignoredFiles select new FileInfo(s)).ToArray();

            this.parsed = parsed;
            this.done = done;

            this.state = ScannerState.NotRunning;

            this.added = updated = skipped = error = removed = total = 0;

            thread.Start();
        }

        private void Run()
        {
            state = ScannerState.Scanning;

            var actions = BuildActionDictionary(ScanForFiles(), existingFiles.Keys);
            foreach (FileInfo file in ignoredFiles)
                actions[file] = Action.Skip;

            total = actions.Count;

            state = ScannerState.Parsing;

            foreach (var file in actions)
                ParseFile(file.Key, file.Value);

            state = ScannerState.Completed;
            if (done != null)
                done(this, new ScanCompletedEventArgs());
        }

        private IEnumerable<FileInfo> ScanForFiles()
        {
            //Get all files in directory
            List<FileInfo> searchFiles = new List<FileInfo>();
            foreach (string ext in extensions)
            {
                FileInfo[] files = directory.GetFiles("*" + ext, searchoption);
                searchFiles.AddRange(files);
            }

            searchFiles = new List<FileInfo>(TrimForExtensions(searchFiles));

            foreach (var file in searchFiles)
                yield return file;
        }
        private IEnumerable<FileInfo> TrimForExtensions(IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                bool ok = false;
                foreach (string ext in extensions)
                    if (file.Extension.ToLower() == ext)
                    {
                        ok = true;
                        break;
                    }
                if (ok)
                    yield return file;
            }
        }

        private Dictionary<FileInfo, Action> BuildActionDictionary(IEnumerable<FileInfo> scanFiles, IEnumerable<FileInfo> existingFiles)
        {
            Dictionary<FileInfo, Action> dictionary = new Dictionary<FileInfo, Action>();

            if (!parseAdd && !parseUpdate && !removeDeadFiles)
                return existingFiles.ToDictionary(x => x, x => Action.Skip);

            List<FileInfo> scan = new List<FileInfo>(scanFiles);
            scan.Sort(ComparePath);

            List<FileInfo> exist = new List<FileInfo>(existingFiles);
            exist.Sort(ComparePath);

            int s = 0;
            int e = 0;
            while (s < scan.Count || s < exist.Count)
            {
                int compare = s == scan.Count ? 1 : e == exist.Count ? -1 : ComparePath(scan[s], exist[e]);
                if (compare < 0)
                {
                    if (parseAdd)
                        dictionary.Add(scan[s], Action.Add);
                    s++;
                }
                else if (compare > 0)
                {
                    if (removeDeadFiles)
                        dictionary.Add(exist[e], Action.Remove);
                    else
                        dictionary.Add(exist[e], Action.Skip);
                    e++;
                }
                else if (compare == 0)
                {
                    if (parseUpdate)
                        dictionary.Add(exist[e], Action.Update);
                    else
                        dictionary.Add(exist[e], Action.Skip);
                    s++; e++;
                }
            }

            return dictionary;
        }

        private void ParseFile(FileInfo file, Action action)
        {
            switch (action)
            {
                case Action.Add:
                case Action.Update:
                    RawTrack rt;
                    if (parser.TryParseTrack(file.FullName, out rt))
                    {
                        if (action == Action.Add)
                            FileParsed(file, rt, FileState.Added);
                        else if (existingFiles[file].Equals(rt))
                            FileParsed(file, FileState.Skipped);
                        else
                            FileParsed(file, rt, FileState.Updated);
                    }
                    else if (action == Action.Update && IsFileLocked(file))
                        FileParsed(file, FileState.Skipped);
                    else
                        FileParsed(file, action == Action.Add ? FileState.AddError : FileState.UpdateError);
                    break;
                case Action.Skip:
                    FileParsed(file, FileState.Skipped);
                    break;
                case Action.Remove:
                    FileParsed(file, FileState.Removed);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file action.");
            }
        }

        private void FileParsed(FileInfo filepath, FileState state)
        {
            RawTrack track = null;
            existingFiles.TryGetValue(filepath, out track);
            FileParsed(filepath, track, state);
        }
        private void FileParsed(FileInfo filepath, RawTrack track, FileState state)
        {
            switch (state)
            {
                case FileState.Added: added++; break;
                case FileState.Updated: updated++; break;
                case FileState.AddError:
                case FileState.UpdateError:
                case FileState.Error: error++; break;
                case FileState.Removed: removed++; break;
                case FileState.Skipped: skipped++; break;
                default: throw new InvalidOperationException("Unknown filestate.");
            }

            if (parsed != null)
                parsed(this, new ScanFileEventArgs(filepath.FullName, track, state));
        }

        private bool IsFileLocked(FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
                return false;

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
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

        private enum Action
        {
            Add,
            Update,
            Skip,
            Remove
        }
    }
}
