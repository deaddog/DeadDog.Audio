namespace DeadDog.Audio.Libraries
{
    public class TrackCollection : LibraryCollectionBase<Track>
    {
        internal TrackCollection() : base(LibraryComparisons.CompareTrackNumbers)
        {
        }
    }
}
