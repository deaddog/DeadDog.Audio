namespace DeadDog.Audio.Libraries
{
    public class ArtistCollection : LibraryCollectionBase<Artist>
    {
        private Artist unknownArtist;

        internal ArtistCollection()
        {
            this.unknownArtist = new Artist(null);
        }

        public Artist UnknownArtist
        {
            get { return unknownArtist; }
        }
        internal override Artist _unknownElement
        {
            get { return unknownArtist; }
        }

        public event ArtistEventHandler ArtistAdded, ArtistRemoved;
        protected override void OnAdded(Artist element)
        {
            ArtistAdded?.Invoke(this, new ArtistEventArgs(element));
        }
        protected override void OnRemoved(Artist element)
        {
            ArtistRemoved?.Invoke(this, new ArtistEventArgs(element));
        }

        protected override string GetName(Artist element)
        {
            return element.Name;
        }
        protected override int Compare(Artist element1, Artist element2)
        {
            return element1.Name.CompareTo(element2.Name);
        }
    }
}
