using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using DeadDog.Audio.Parsing;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScan
    {
        private IDataParser parser;
        private Library library;

        private string[] extensions;
        private Dictionary<FileInfo, RawTrack> existingFiles;
        private FileInfo[] ignoredFiles;

        private event ScanFileEventHandler parsed;
        private event ScanCompletedEventHandler done;

        internal AudioScan(DirectoryInfo directory, SearchOption searchoption,
            bool parseAdd, bool parseUpdate, bool removeDeadFiles, IDataParser parser,
            ScanFileEventHandler parsed, ScanCompletedEventHandler done)
        {
            this.directory = directory;
            this.searchoption = searchoption;

            this.parseAdd = parseAdd;
            this.parseUpdate = parseUpdate;
            this.removeDeadFiles = removeDeadFiles;

            this.parser = parser;
            this.library = null;

            this.extensions = new string[] { };
            this.existingFiles = new Dictionary<FileInfo, RawTrack>();
            this.ignoredFiles = new FileInfo[] { };

            this.parsed = parsed;
            this.done = done;

            this.state = ScannerState.NotRunning;

            this.added = updated = skipped = error = removed = total = 0;
        }

        internal Library Library
        {
            set { this.library = library; }
        }

        internal IEnumerable<string> Extensions
        {
            set { extensions = value.ToArray(); }
        }
        internal IEnumerable<RawTrack> ExistingFiles
        {
            set { existingFiles = value.ToDictionary(rt => rt.File); }
        }
        internal IEnumerable< string> IgnoredFiles
        {
            set { ignoredFiles = (from s in value select new FileInfo(s)).ToArray(); }
        }

        internal void Start()
        {
            new Thread(() =>
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
            }).Start();

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
                foreach (string ext in extensions)
                    if (file.Extension.ToLower() == ext)
                    {
                        yield return file;
                        break;
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

            if (library != null)
                switch (state)
                {
                    case FileState.Added: library.AddTrack(track); break;
                    case FileState.Updated: library.UpdateTrack(track); break;
                    case FileState.Removed: library.RemoveTrack(filepath.FullName); break;
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

        private int ComparePath(FileInfo x, FileInfo y)
        {
            return x.FullName.CompareTo(y.FullName);
        }

        private enum Action
        {
            Add,
            Update,
            Skip,
            Remove
        }
    }
}
