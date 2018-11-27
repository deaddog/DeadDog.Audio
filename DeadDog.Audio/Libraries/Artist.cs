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

            Albums = new LibraryCollection<Album>(LibraryComparisons.Title);
            Tracks = new LibraryCollection<Track>(LibraryComparisons.Number);

            Name = name ?? string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}
