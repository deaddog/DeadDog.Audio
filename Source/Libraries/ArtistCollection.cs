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
    }
}
