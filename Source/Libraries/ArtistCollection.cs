namespace DeadDog.Audio.Libraries
{
    public class ArtistCollection : LibraryCollection<Artist>
    {
        internal ArtistCollection() : base(LibraryComparisons.CompareArtistNames)
        {
        }
    }
}
