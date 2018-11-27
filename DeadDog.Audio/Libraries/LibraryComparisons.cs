namespace DeadDog.Audio.Libraries
{
    public static class LibraryComparisons
    {
        private static int CompareNullableStrings(string s1, string s2)
        {
            if (s1 == null)
                return s2 == null ? 0 : -1;
            else if (s2 == null)
                return 1;

            return s1.CompareTo(s2);
        }

        public static int CompareTrackNumbers(Track track1, Track track2)
        {
            int? v1 = track1.Tracknumber, v2 = track2.Tracknumber;
            if (v1.HasValue)
                return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
            else
                return v2.HasValue ? -1 : 0;
        }
        public static int CompareAlbumTitlesTrackNumbers(Track track1, Track track2)
        {
            int albumCompare = CompareAlbumTitles(track1?.Album, track2?.Album);
            if (albumCompare != 0)
                return albumCompare;

            return CompareTrackNumbers(track1, track2);
        }
        public static int CompareArtistNameAlbumTitlesTrackNumbers(Track track1, Track track2)
        {
            int artistCompare = CompareArtistNames(track1?.Artist, track2?.Artist);
            if (artistCompare != 0)
                return artistCompare;

            int albumCompare = CompareAlbumTitles(track1?.Album, track2?.Album);
            if (albumCompare != 0)
                return albumCompare;

            return CompareTrackNumbers(track1, track2);
        }

        public static int CompareAlbumTitles(Album album1, Album album2)
        {
            return CompareNullableStrings(album1?.Title, album2?.Title);
        }
        public static int CompareArtistNamesAlbumTitles(Album album1, Album album2)
        {
            int artistCompare = CompareArtistNames(album1?.Artist, album2?.Artist);
            if (artistCompare != 0)
                return artistCompare;

            return CompareAlbumTitles(album1, album2);
        }

        public static int CompareArtistNames(Artist artist1, Artist artist2)
        {
            return CompareNullableStrings(artist1?.Name, artist2?.Name);
        }
    }
}
