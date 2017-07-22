using DeadDog.Audio.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeadDog.Audio.Scan
{
    public static class AudioScanner
    {
        public static async Task<ScannedFile[]> ScanDirectory(ScannerSettings settings, IProgress<ScanProgress> progress = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            progress?.Report(new ScanProgress(ScannerStates.Scanning, 0, 0, 0, 0, 0, 0));

            var existingTracks = settings.UseCache ?
                GetExistingTracks(settings.CachePath) :
                new RawTrack[0];

            var filesOnDisc = settings.IncludeNewFiles || settings.IncludeFileUpdates || settings.RemoveMissingFiles ?
                GetMediaFiles(settings.Directory, settings.SearchOption, settings.Extensions) :
                existingTracks.Select(x => x.Filepath);

            var fileActions = GetFileActions(filesOnDisc, existingTracks).ToList();

            var resultFiles = new List<ScannedFile>();
            var state = new ScanProgress(ScannerStates.Parsing, 0, 0, 0, 0, 0, fileActions.Count);

            await Task.Run(() =>
            {
                foreach (var f in fileActions)
                {
                    var file = ScanFile(f, settings);

                    progress?.Report(state = GetNewProgress(state, file));
                    resultFiles.Add(file);
                }
            });

            progress?.Report(new ScanProgress(ScannerStates.Completed, state.Added, state.Updated, state.Skipped, state.Errors, state.Removed, state.Total));
            return resultFiles.ToArray();
        }

        private static ScanProgress GetNewProgress(ScanProgress prev, ScannedFile file)
        {
            switch (file.Action)
            {
                case FileActions.Added: return new ScanProgress(prev.State, prev.Added + 1, prev.Updated, prev.Skipped, prev.Errors, prev.Removed, prev.Total, file);
                case FileActions.Updated: return new ScanProgress(prev.State, prev.Added, prev.Updated + 1, prev.Skipped, prev.Errors, prev.Removed, prev.Total, file);
                case FileActions.Error:
                case FileActions.AddError:
                case FileActions.UpdateError: return new ScanProgress(prev.State, prev.Added, prev.Updated, prev.Skipped, prev.Errors + 1, prev.Removed, prev.Total, file);
                case FileActions.Removed: return new ScanProgress(prev.State, prev.Added, prev.Updated, prev.Skipped, prev.Errors, prev.Removed + 1, prev.Total, file);
                case FileActions.Skipped: return new ScanProgress(prev.State, prev.Added, prev.Updated, prev.Skipped + 1, prev.Errors, prev.Removed, prev.Total, file);

                default:
                    throw new InvalidCastException($"Unknown {nameof(FileActions)} in progress update. The value \"{file.Action}\" is not defined.");
            }
        }

        private static IEnumerable<string> GetMediaFiles(string directory, SearchOption searchOption, IEnumerable<string> extensions)
        {
            foreach (var ext in extensions)
                foreach (var file in Directory.GetFiles(directory, "*" + ext, searchOption))
                {
                    if (Path.GetExtension(file).Equals(ext, StringComparison.OrdinalIgnoreCase))
                        yield return file;
                }
        }
        private static IEnumerable<RawTrack> GetExistingTracks(string filepath)
        {
            if (!File.Exists(filepath))
                return new RawTrack[0];

            var tracks = new List<RawTrack>();

            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                int count = fs.ReadInt32();

                for (int i = 0; i < count; i++)
                    tracks.Add(RawTrack.FromStream(fs));
            }

            return tracks;
        }
        private static IEnumerable<ScannedFile> GetFileActions(IEnumerable<string> scanFiles, IEnumerable<RawTrack> existingTracks)
        {
            var scan = new List<string>(scanFiles);
            scan.Sort(StringComparer.OrdinalIgnoreCase);

            var exist = new List<RawTrack>(existingTracks);
            exist.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Filepath, y.Filepath));

            int s = 0;
            int e = 0;
            while (s < scan.Count || e < exist.Count)
            {
                int compare = s == scan.Count ? 1 : e == exist.Count ? -1 : StringComparer.OrdinalIgnoreCase.Compare(scan[s], exist[e].Filepath);
                if (compare < 0)
                    yield return new ScannedFile(scan[s++], null, FileActions.Added);
                else if (compare > 0)
                    yield return new ScannedFile(exist[e].Filepath, exist[e++], FileActions.Removed);
                else
                    yield return new ScannedFile(scan[s++], exist[e++], FileActions.Updated);
            }
        }

        private static ScannedFile ScanFile(ScannedFile file, ScannerSettings settings)
        {
            switch (file.Action)
            {
                case FileActions.Added:
                    {
                        if (!settings.IncludeNewFiles)
                            return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.Skipped);
                        else if (settings.Parser.TryParseTrack(file.Filepath, out var track))
                            return new ScannedFile(file.Filepath, track, FileActions.Added);
                        else
                            return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.AddError);
                    }

                case FileActions.Updated:
                    {
                        if (!settings.IncludeFileUpdates)
                            return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.Skipped);
                        else if (settings.Parser.TryParseTrack(file.Filepath, out var track))
                            return new ScannedFile(file.Filepath, track, FileActions.Updated);
                        else
                            return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.UpdateError);
                    }

                case FileActions.Removed:
                    if (!settings.RemoveMissingFiles)
                        return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.Skipped);
                    else
                        return new ScannedFile(file.Filepath, file.MediaInfo, FileActions.Removed);

                default:
                    throw new InvalidDataException($"A file cannot have the {file.Action} action pre-parse.");
            }
        }
    }
}
