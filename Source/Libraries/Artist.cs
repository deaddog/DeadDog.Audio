namespace DeadDog.Audio.Libraries
{
    public class Artist
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

        public override string ToString()
        {
            return Name;
        }
    }
}
