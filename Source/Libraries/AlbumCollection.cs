namespace DeadDog.Audio.Libraries
{
    public class AlbumCollection : LibraryCollectionBase<Album>
    {
        private Album unknownAlbum;

        internal AlbumCollection() : base(LibraryComparisons.CompareAlbumTitles)
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
