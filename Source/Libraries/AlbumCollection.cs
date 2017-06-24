namespace DeadDog.Audio.Libraries
{
    public class AlbumCollection : LibraryCollectionBase<Album>
    {
        internal AlbumCollection() : base(LibraryComparisons.CompareAlbumTitles)
        {
        }
    }
}
