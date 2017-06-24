namespace DeadDog.Audio.Libraries
{
    public class AlbumCollection : LibraryCollection<Album>
    {
        internal AlbumCollection() : base(LibraryComparisons.CompareAlbumTitles)
        {
        }
    }
}
