namespace DeadDog.Audio.Libraries
{
    public class ArtistCollection : LibraryCollectionBase<Artist>
    {
        internal ArtistCollection() : base(LibraryComparisons.CompareArtistNames)
        {
        }
    }
}
