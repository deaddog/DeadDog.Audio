namespace DeadDog.Audio.Libraries
{
    public class ArtistCollection : LibraryCollectionBase<Artist>
    {
        private Artist unknownArtist;

        internal ArtistCollection() : base((a1, a2) => a1.Name.CompareTo(a2.Name))
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
    }
}
