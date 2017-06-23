namespace DeadDog.Audio.Libraries
{
    public class Artist
    {
        #region Properties

        private bool isunknown;
        public bool IsUnknown
        {
            get { return isunknown; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private AlbumCollection albums;
        public AlbumCollection Albums
        {
            get { return albums; }
        }

        #endregion

        internal Artist(string name)
        {
            this.isunknown = name == null;
            this.albums = new AlbumCollection();
            this.albums.UnknownAlbum.Artist = this;

            this.name = name ?? string.Empty;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
