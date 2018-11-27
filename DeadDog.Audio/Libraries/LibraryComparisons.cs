using System;

namespace DeadDog.Audio.Libraries
{
    public static class LibraryComparisons
    {
        public static Comparison<Track> CompareBy(Comparison<Track> comparison) => comparison;
        public static Comparison<Album> CompareBy(Comparison<Album> comparison) => comparison;
        public static Comparison<Artist> CompareBy(Comparison<Artist> comparison) => comparison;

        public static Comparison<T> ThenBy<T>(this Comparison<T> comparison, Comparison<T> nextComparison)
        {
            return (t1, t2) =>
            {
                var compare = comparison(t1, t2);

                if (compare == 0)
                    compare = nextComparison(t1, t2);

                return compare;
            };
        }

        public static int Number(Track track1, Track track2)
        {
            int? v1 = track1.Tracknumber, v2 = track2.Tracknumber;
            if (v1.HasValue)
                return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
            else
                return v2.HasValue ? -1 : 0;
        }
        public static int Title(Track track1, Track track2) => CompareNullableStrings(track1.Title, track2.Title);

        public static int Artist(Track track1, Track track2) => CompareNullableStrings(track1.Artist.Name, track2.Artist.Name);
        public static int Album(Track track1, Track track2) => CompareNullableStrings(track1.Album.Title, track2.Album.Title);
        public static int ArtistOrAlbum(Track track1, Track track2)
        {
            if (track1.Album.HasArtist)
            {
                if (track2.Album.HasArtist)
                    return CompareBy(Artist).ThenBy(Album)(track1, track2);
                else
                    return CompareNullableStrings(track1.Album.Artist.Name, track2.Album.Title);
            }
            else if (track2.Album.HasArtist)
                return -ArtistOrAlbum(track2, track1);
            else
                return Album(track1, track2);
        }

        public static int Title(Album album1, Album album2) => CompareNullableStrings(album1?.Title, album2?.Title);
        public static int Name(Artist artist1, Artist artist2) => CompareNullableStrings(artist1?.Name, artist2?.Name);
        public static int ArtistName(Album album1, Album album2) => Name(album1?.Artist, album2?.Artist);

        private static int CompareNullableStrings(string s1, string s2)
        {
            if (s1 == null)
                return s2 == null ? 0 : -1;
            else if (s2 == null)
                return 1;

            return s1.CompareTo(s2);
        }
    }
}
