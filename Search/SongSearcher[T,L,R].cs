using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    /// <summary>
    /// OBSOLETE - Use <see cref="PlaylistSearcher"/> or <see cref="LibrarySearcher"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of tracks in use by library.</typeparam>
    /// <typeparam name="L">The type of albums in use by library.</typeparam>
    /// <typeparam name="R">The type of artists in use by library.</typeparam>
    public struct SongSearcher<T, L, R>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private string[] searchstring;
        private SearchMethods method;

        public SongSearcher(SearchMethods method, string searchstring)
            : this(method, searchstring.Trim().Split(' '))
        {
        }
        public SongSearcher(SearchMethods method, params string[] searchstring)
        {
            this.searchstring = searchstring;
            for (int i = 0; i < searchstring.Length; i++)
                searchstring[i] = searchstring[i].ToLower();
            this.method = method;
        }

        public string[] Searchstring
        {
            get { return searchstring; }
        }
        public SearchMethods Method
        {
            get { return method; }
        }

        /// <summary>
        /// OBSOLETE - Use <see cref="PlaylistSearcher"/> instead.
        /// </summary>
        /// <param name="playlist">The playlist to search.</param>
        /// <returns>A new list containing the result.</returns>
        public List<PlaylistEntry<T>> SearchPlaylist(Playlist<T> playlist)
        {
            List<PlaylistEntry<T>> results = new List<PlaylistEntry<T>>();
            for (int i = 0; i < playlist.Count; i++)
                if (Predicate(playlist[i]))
                    results.Add(playlist[i]);
            return results;
        }
        private bool Predicate(PlaylistEntry<T> entry)
        {
            if (searchstring.Length == 0)
                return true;
            else if (method == SearchMethods.ContainsAll)
            {
                for (int i = 0; i < searchstring.Length; i++)
                    if (!(entry.Track.Contains(searchstring[i]) ||
                        entry.Track.Album.Contains(searchstring[i]) ||
                        entry.Track.Album.Artist.Contains(searchstring[i])))
                        return false;
                return true;
            }
            else // method == SearchMethod.ContainsAny
            {
                for (int i = 0; i < searchstring.Length; i++)
                    if (entry.Track.Contains(searchstring[i]) ||
                        entry.Track.Album.Contains(searchstring[i]) ||
                        entry.Track.Album.Artist.Contains(searchstring[i]))
                        return true;
                return false;
            }
        }

        /// <summary>
        /// OBSOLETE - Use <see cref="LibrarySearcher"/> instead.
        /// </summary>
        /// <param name="library">The library to search.</param>
        /// <returns>A new list containing the result.</returns>
        public List<T> SearchLibrary(Library<T, L, R> library)
        {
            List<string> complete = new List<string>(searchstring);
            List<T> result = new List<T>();

            foreach (T track in library.Artists.UnknownArtist.Albums.UnknownAlbum.Tracks)
                if (track.SearchForStrings(method, complete.ToArray()))
                    result.Add(track);

            foreach (R artist in library.Artists)
            {
                List<string> artistResults = new List<string>(complete);
                if (artist.SearchForStrings(method, artistResults))
                    AddTracks(artist, result);
                else
                {
                    foreach (T track in artist.Albums.UnknownAlbum.Tracks)
                        if (track.SearchForStrings(method, artistResults.ToArray()))
                            result.Add(track);

                    foreach (L album in artist.Albums)
                    {
                        List<string> albumResults = new List<string>(artistResults);
                        if (album.SearchForStrings(method, albumResults))
                            result.AddRange(album.Tracks);
                        else
                            foreach (T track in album.Tracks)
                                if (track.SearchForStrings(method, albumResults.ToArray()))
                                    result.Add(track);
                    }
                }
            }

            return result;
        }

        private void AddTracks(R artist, List<T> list)
        {
            list.AddRange(artist.Albums.UnknownAlbum.Tracks);
            foreach (L album in artist.Albums)
                list.AddRange(album.Tracks);
        }
    }
}
