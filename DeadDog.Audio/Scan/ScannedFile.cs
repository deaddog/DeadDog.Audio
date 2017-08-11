namespace DeadDog.Audio.Scan
{
    public struct ScannedFile
    {
        public ScannedFile(string filepath, RawTrack mediaInfo, FileActions action)
        {
            Filepath = filepath;
            MediaInfo = mediaInfo;
            Action = action;
        }

        public string Filepath { get; }
        public RawTrack MediaInfo { get; }
        public FileActions Action { get; }
    }
}
