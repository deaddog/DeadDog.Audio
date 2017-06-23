namespace DeadDog.Audio.Libraries
{
    public class Track
    {
        #region Fields and properties

        private System.IO.FileInfo file;
        public bool FileExist
        {
            get { file.Refresh(); return file.Exists; }
        }
        public string FilePath
        {
            get { return file.FullName; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            internal set { title = value; }
        }

        private int? tracknumber;
        public int? Tracknumber
        {
            get { return tracknumber; }
            internal set { tracknumber = value; }
        }

        private Album album;
        public Album Album
        {
            get { return album; }
            internal set { album = value; }
        }

        private Artist artist;
        public Artist Artist
        {
            get { return artist; }
            internal set { artist = value; }
        }

        #endregion

        internal Track(RawTrack trackinfo)
        {
            this.file = new System.IO.FileInfo(trackinfo.Filepath);
            this.album = null;
            this.artist = null;
            this.title = trackinfo.TrackTitle;
            this.tracknumber = trackinfo.TrackNumber;
        }

        public override string ToString()
        {
            return (tracknumber == null ? "" : "#" + tracknumber + " ") + title;
        }
    }
}
