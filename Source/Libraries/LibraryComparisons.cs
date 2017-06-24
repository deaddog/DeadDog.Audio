namespace DeadDog.Audio.Libraries
{
    internal static class LibraryComparisons
    {
        public static int CompareTrackNumbers(Track element1, Track element2)
        {
            int? v1 = element1.Tracknumber, v2 = element2.Tracknumber;
            if (v1.HasValue)
                return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
            else
                return v2.HasValue ? -1 : 0;
        }
        public static int CompareAlbumTitles(Album album1, Album album2)
        {
            return album1.Title.CompareTo(album2.Title);
        }
        public static int CompareArtistNames(Artist artist1, Artist artist2)
        {
            return artist1.Name.CompareTo(artist2.Name);
        }
    }
}
