using System;
using System.IO;

namespace DeadDog.Audio.Scan
{
    public class ScannerSettings
    {
        public ScannerSettings(string directory, SearchOption searchOption, string cachePath = null)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
            SearchOption = searchOption;
            CachePath = cachePath;
        }

        public string Directory { get; }
        public SearchOption SearchOption { get; }
        public string CachePath { get; }
        public bool UseCache => CachePath != null;
    }
}
