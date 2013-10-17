﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public static class Searching
    {
        public static IEnumerable<T> Search<T>(this IEnumerable<T> list, PredicateString<T> predicate, SearchMethods method, string searchstring)
        {
            return Search(list, predicate, method, splitString(searchstring));
        }
        public static IEnumerable<T> Search<T>(this IEnumerable<T> list, PredicateString<T> predicate, SearchMethods method, params string[] searchstring)
        {
            string[] search = searchstring;
            for (int i = 0; i < search.Length; i++)
                search[i] = search[i].ToLower();

            if (search.Length == 0)
            {
                foreach (T element in list)
                    yield return element;
            }
            else
            {
                foreach (T element in list)
                    if (compareElement(element, predicate, method, search))
                        yield return element;
            }
        }

        public static IEnumerable<T> Search<T, R>(this IEnumerable<T> list, Converter<T, R> convert, PredicateString<R> predicate, SearchMethods method, string searchstring)
        {
            return Search(list, convert, predicate, method, splitString(searchstring));
        }
        public static IEnumerable<T> Search<T, R>(this IEnumerable<T> list, Converter<T, R> convert, PredicateString<R> predicate, SearchMethods method, params string[] searchstring)
        {
            return Search(list, (t, s) => predicate(convert(t), s), method, searchstring);
        }

        public static IEnumerable<PlaylistEntry<T>> Search<T>(this IPlaylist<T> playlist, PredicateString<T> predicate, SearchMethods method, string searchstring)
        {
            return Search(playlist, predicate, method, splitString(searchstring));
        }
        public static IEnumerable<PlaylistEntry<T>> Search<T>(this IPlaylist<T> playlist, PredicateString<T> predicate, SearchMethods method, params string[] searchstring)
        {
            return Search(playlist, t => t.Track, predicate, method, searchstring);
        }

        public static IEnumerable<PlaylistEntry<Track>> Search(this IPlaylist<Track> playlist, SearchMethods method, string searchstring)
        {
            return Search(playlist, method, splitString(searchstring));
        }
        public static IEnumerable<PlaylistEntry<Track>> Search(this IPlaylist<Track> playlist, SearchMethods method, params string[] searchstring)
        {
            return Search(playlist, ContainedInTitleArtistAlbum, method, searchstring);
        }

        private static string[] splitString(string searchstring)
        {
            return searchstring.Trim().Split(' ');
        }

        private static bool compareElement<T>(T track, PredicateString<T> pre, SearchMethods method, string[] searchstring)
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
            return Match(track.Artist, searchstring) || Match(track.Album, searchstring) || Match(track, searchstring);
        }
        public static bool ContainedInTitle(Track track, string searchstring)
        {
            return Match(track, searchstring);
        }

        public static bool Match(Artist artist, string searchstring)
        {
            return artist.Name != null ? artist.Name.ToLower().Contains(searchstring) : false;
        }
        public static bool Match(Album album, string searchstring)
        {
            return album.Title != null ? album.Title.ToLower().Contains(searchstring) : false;
        }
        public static bool Match(Track track, string searchstring)
        {
            return track.Title != null ? track.Title.ToLower().Contains(searchstring) : false;
        }

        public static bool ContainedInTitleArtistAlbum(RawTrack track, string searchstring)
        {
            return (track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring))
                || (track.AlbumTitle == null ? false : track.AlbumTitle.ToLower().Contains(searchstring))
                || (track.ArtistName == null ? false : track.ArtistName.ToLower().Contains(searchstring));
        }
        public static bool ContainedInTitle(RawTrack track, string searchstring)
        {
            return track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring);
        }
    }
}
