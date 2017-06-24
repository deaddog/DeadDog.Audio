using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

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
            private set
            {
                if (_artist != value)
                {
                    if (_artist != null)
                        _artist.Albums.Remove(this);

                    bool hasArtistChanged = _artist == null || value == null;
                    _artist = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Artist)));
                    if (hasArtistChanged)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasArtist)));

                    if (value != null)
                        value.Albums.Add(this);
                }
            }
        }
        public bool HasArtist => _artist != null;

        internal Album(string title)
        {
            IsUnknown = title == null;
            Tracks = new TrackCollection();
            Tracks.CollectionChanged += TracksCollectionChanged;

            Title = title ?? string.Empty;
            _artist = null;
        }

        private void TracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Track t in e.NewItems)
                    t.PropertyChanged += TrackPropertyChanged;
                Artist = Tracks.AllSameOrDefault(x => x.Artist)?.Artist;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Track t in e.OldItems)
                    t.PropertyChanged -= TrackPropertyChanged;
                Artist = Tracks.AllSameOrDefault(x => x.Artist)?.Artist;
            }
        }
        private void TrackPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Track.Artist))
                Artist = Tracks.AllSameOrDefault(x => x.Artist)?.Artist;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Title;
        }
    }
}
