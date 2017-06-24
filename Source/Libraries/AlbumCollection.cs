namespace DeadDog.Audio.Libraries
{
    public class AlbumCollection : LibraryCollectionBase<Album>
    {
        private Album unknownAlbum;

        internal AlbumCollection() : base((a1, a2) => a1.Title.CompareTo(a2.Title))
        {
            this.unknownAlbum = new Album(null);
        }

        public Album UnknownAlbum
        {
            get { return unknownAlbum; }
        }
        internal override Album _unknownElement
        {
            get { return unknownAlbum; }
        }
    }
}
