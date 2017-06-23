namespace DeadDog.Audio.Libraries
{
    public class Album
    {
        #region Properties

        private bool isunknown;
        public bool IsUnknown
        {
            get { return isunknown; }
        }

        private string title;
        public string Title
        {
            get { return title; }
        }

        private TrackCollection tracks;
        public TrackCollection Tracks
        {
            get { return tracks; }
        }

        // This is correct! - Artist should NOT be a constructor argument.
        private Artist artist = null;
        public Artist Artist
        {
            get { return artist; }
            internal set { artist = value; }
        }

        public bool HasArtist
        {
            get { return artist != null; }
        }

        #endregion

        internal Album(string album)
        {
            this.isunknown = album == null;
            this.tracks = new TrackCollection();

            this.title = album ?? string.Empty;
        }

        public override string ToString()
        {
            return title;
        }
    }
}
