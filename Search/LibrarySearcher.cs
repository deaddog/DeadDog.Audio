using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio
{
    public class LibrarySearcher<T, L, R>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private Library<T, L, R> library;

        public LibrarySearcher(Library<T, L, R> library)
        {
            this.library = library;
        }

        public T[] Search(SearchMethods method, string searchstring)
        {
            return Search(method, searchstring.Trim().Split(' '));
        }
        public T[] Search(SearchMethods method, params string[] searchstring)
        {
            string[] search = searchstring;
            for (int i = 0; i < search.Length; i++)
                search[i] = search[i].ToLower();

            List<string> complete = new List<string>(search);
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

            return result.ToArray();
        }

        private void AddTracks(R artist, List<T> list)
        {
            list.AddRange(artist.Albums.UnknownAlbum.Tracks);
            foreach (L album in artist.Albums)
                list.AddRange(album.Tracks);
        }
    }
}
