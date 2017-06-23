namespace DeadDog.Audio.Libraries
{
    public class Album
    {
        private Artist _artist;

        public bool IsUnknown { get; }

        public string Title { get; }

        public TrackCollection Tracks { get; }

        public Artist Artist
        {
            get { return _artist; }
            internal set { _artist = value; }
        }
        public bool HasArtist => _artist != null;

        internal Album(string album)
        {
            IsUnknown = album == null;
            Tracks = new TrackCollection();

            Title = album ?? string.Empty;
            _artist = null;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
