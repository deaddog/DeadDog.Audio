using DeadDog.Audio.Parsing;
using System;
using System.IO;

namespace DeadDog.Audio.Scan
{
    public class ScannerSettings
    {
        private IMediaParser _parser = MediaParser.GetDefault(true);
        private string[] _extensions = new string[] { "ogg", "mp3", "flac" };

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

        public IMediaParser Parser
        {
            get => _parser;
            set => _parser = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IncludeNewFiles { get; set; } = true;
        public bool IncludeFileUpdates { get; set; } = true;
        public bool RemoveMissingFiles { get; set; } = true;

        public string[] Extensions
        {
            get => _extensions;
            set => _extensions = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
