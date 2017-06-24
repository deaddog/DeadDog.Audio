using System.ComponentModel;

namespace DeadDog.Audio.Libraries
{
    public class Artist : INotifyPropertyChanged
    {
        public bool IsUnknown { get; }

        public string Name { get; }

        public LibraryCollection<Album> Albums { get; }
        public LibraryCollection<Track> Tracks { get; }

        internal Artist(string name)
        {
            IsUnknown = name == null;

            Albums = new LibraryCollection<Album>(LibraryComparisons.CompareAlbumTitles);
            Tracks = new LibraryCollection<Track>(LibraryComparisons.CompareTrackNumbers);

            Name = name ?? string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}
