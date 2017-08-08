using DeadDog.Audio.Libraries;
using System;

namespace DeadDog.Audio.Scan
{
    public class LibraryUpdateProgress : Progress<ScanProgress>
    {
        public LibraryUpdateProgress(Library library) : base(x => UpdateLibrary(library, x))
        {
            if (library == null)
                throw new ArgumentNullException(nameof(library));
        }

        public static void UpdateLibrary(Library library, ScanProgress progress)
        {
            if (!progress.ScannedFile.HasValue)
                return;

            var scannedFile = progress.ScannedFile.Value;

            switch (scannedFile.Action)
            {
                case FileActions.Added:
                    library.AddTrack(scannedFile.MediaInfo);
                    break;

                case FileActions.Updated:
                    library.UpdateTrack(scannedFile.MediaInfo);
                    break;

                case FileActions.Removed:
                    library.RemoveTrack(scannedFile.Filepath);
                    break;
            }
        }
    }
}
