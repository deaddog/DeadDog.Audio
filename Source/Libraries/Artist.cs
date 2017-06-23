using System.ComponentModel;

namespace DeadDog.Audio.Libraries
{
    public class Artist : INotifyPropertyChanged
    {
        public bool IsUnknown { get; }

        public string Name { get; }

        public AlbumCollection Albums { get; }

        internal Artist(string name)
        {
            IsUnknown = name == null;
            Albums = new AlbumCollection();
            Albums.UnknownAlbum.Artist = this;

            Name = name ?? string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}
