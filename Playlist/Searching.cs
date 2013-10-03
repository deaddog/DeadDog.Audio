using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public static class Searching
    {
        public static IEnumerable<PlaylistEntry<T>> Search<T>(this IPlaylist<T> playlist, SearchMethods method, PredicateString<T> predicate, string searchstring)
        {
            return Search(playlist, method, predicate, searchstring.Trim().Split(' '));
        }
        public static IEnumerable<PlaylistEntry<T>> Search<T>(this IPlaylist<T> playlist, SearchMethods method, PredicateString<T> predicate, params string[] searchstring)
        {
            string[] search = searchstring;
            for (int i = 0; i < search.Length; i++)
                search[i] = search[i].ToLower();

            if (search.Length == 0)
            {
                foreach (PlaylistEntry<T> track in playlist)
                    yield return track;
            }
            else
            {
                foreach(PlaylistEntry<T> track in playlist)
                    if (test(track.Track, method, search, predicate))
                        yield return track;
            }
        }

        public static IEnumerable<PlaylistEntry<Track>> Search(this IPlaylist<Track> playlist, SearchMethods method, string searchstring)
        {
            return Search(playlist, method, ContainedInTitleArtistAlbum, searchstring);
        }
        public static IEnumerable<PlaylistEntry<Track>> Search(this IPlaylist<Track> playlist, SearchMethods method, params string[] searchstring)
        {
            return Search(playlist, method, ContainedInTitleArtistAlbum, searchstring);
        }

        private static bool test<T>(T track, SearchMethods method, string[] searchstring, PredicateString<T> pre)
        {
            switch (method)
            {
                case SearchMethods.ContainsAny:
                    for (int i = 0; i < searchstring.Length; i++)
                        if (pre(track, searchstring[i]))
                            return true;
                    return false;
                case SearchMethods.ContainsAll:
                    for (int i = 0; i < searchstring.Length; i++)
                        if (!pre(track, searchstring[i]))
                            return false;
                    return true;

                default:
                    throw new InvalidOperationException("Unknown search method.");
            }
        }

        public static bool ContainedInTitleArtistAlbum(Track track, string searchstring)
        {
            return track.Title.ToLower().Contains(searchstring) || 
                   track.AlbumTitle.ToLower().Contains(searchstring) || 
                   track.ArtistName.ToLower().Contains(searchstring);
        }
        public static bool ContainedInTitleArtist(Track track, string searchstring)
        {
            return track.Title.ToLower().Contains(searchstring) ||
                   track.ArtistName.ToLower().Contains(searchstring);
        }
        public static bool ContainedInTitle(Track track, string searchstring)
        {
            return track.Title.ToLower().Contains(searchstring);
        }

        public static bool ContainedInTitleArtistAlbum(RawTrack track, string searchstring)
        {
            return (track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring))
                || (track.AlbumTitle == null ? false : track.AlbumTitle.ToLower().Contains(searchstring))
                || (track.ArtistName == null ? false : track.ArtistName.ToLower().Contains(searchstring));
        }
        public static bool ContainedInTitleArtist(RawTrack track, string searchstring)
        {
            return (track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring))
                || (track.ArtistName == null ? false : track.ArtistName.ToLower().Contains(searchstring));
        }
        public static bool ContainedInTitle(RawTrack track, string searchstring)
        {
            return track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring);
        }
    }
}
