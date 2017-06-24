namespace DeadDog.Audio.Libraries
{
    public class TrackCollection : LibraryCollection<Track>
    {
        internal TrackCollection() : base(LibraryComparisons.CompareTrackNumbers)
        {
        }
    }
}
