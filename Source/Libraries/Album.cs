using System.ComponentModel;

namespace DeadDog.Audio.Libraries
{
    public class Album : INotifyPropertyChanged
    {
        private Artist _artist;

        public bool IsUnknown { get; }

        public string Title { get; }

        public TrackCollection Tracks { get; }

        public Artist Artist
        {
            get { return _artist; }
            internal set
            {
                if (_artist != value)
                {
                    bool hasArtistChanged = _artist == null || value == null;
                    _artist = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Artist)));
                    if (hasArtistChanged)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasArtist)));
                }
            }
        }
        public bool HasArtist => _artist != null;

        internal Album(string album)
        {
            IsUnknown = album == null;
            Tracks = new TrackCollection();

            Title = album ?? string.Empty;
            _artist = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Title;
        }
    }
}
