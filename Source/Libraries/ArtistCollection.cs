namespace DeadDog.Audio.Libraries
{
    public class ArtistCollection : LibraryCollectionBase<Artist>
    {
        private Artist unknownArtist;

        internal ArtistCollection() : base(LibraryComparisons.CompareArtistNames)
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
