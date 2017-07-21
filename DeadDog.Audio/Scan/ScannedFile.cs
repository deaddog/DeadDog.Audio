namespace DeadDog.Audio.Scan
{
    public struct ScannedFile
    {
        public ScannedFile(string filepath, RawTrack mediaInfo, FileActions action, ScanProgress scanProgress)
        {
            Filepath = filepath;
            MediaInfo = mediaInfo;
            Action = action;
            ScanProgress = scanProgress;
        }

        public string Filepath { get; }
        public RawTrack MediaInfo { get; }
        public FileActions Action { get; }

        public ScanProgress ScanProgress { get; }
    }
}
