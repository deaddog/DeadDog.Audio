namespace DeadDog.Audio.Libraries
{
    public class AlbumCollection : LibraryCollectionBase<Album>
    {
        private Album unknownAlbum;

        internal AlbumCollection()
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

        protected override string GetName(Album element)
        {
            return element.Title;
        }
        protected override int Compare(Album element1, Album element2)
        {
            return element1.Title.CompareTo(element2.Title);
        }
    }
}
