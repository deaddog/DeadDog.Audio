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

        public event AlbumEventHandler AlbumAdded, AlbumRemoved;
        protected override void OnAdded(Album element)
        {
            AlbumAdded?.Invoke(this, new AlbumEventArgs(element));
        }
        protected override void OnRemoved(Album element)
        {
            AlbumRemoved?.Invoke(this, new AlbumEventArgs(element));
        }
    }
}
