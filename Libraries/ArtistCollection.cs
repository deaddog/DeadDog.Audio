using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Artist
    {
        public class ArtistCollection : IEnumerable<Artist>, ITracks
        {
            private Library library;

            private Artist unknownArtist;
            private List<Artist> artists;

            public ArtistCollection(Library library, Artist unknownArtist)
            {
                this.library = library;
                this.unknownArtist = unknownArtist;
                this.artists = new List<Artist>();
            }

            internal Artist CreateArtist(RawTrack trackinfo, ArtistFactory artistFactory)
            {
                Artist artist = artistFactory.CreateArtist(trackinfo, this.library);

                int index = artists.BinarySearch(artist, (x, y) => x.name.CompareTo(y.name));
                if (~index < 0)
                    artists.Add(artist);
                else
                    artists.Insert(~index, artist);

                return artist;
            }
            internal void RemoveArtist(Artist artist)
            {
                artists.Remove(artist);
            }

            public Artist UnknownArtist
            {
                get { return unknownArtist; }
            }

            public int Count
            {
                get { return artists.Count; }
            }
            public Artist this[int index]
            {
                get { return artists[index]; }
            }
            public Artist this[string artistname]
            {
                get
                {
                    if (artistname == null || artistname.Length == 0)
                        return unknownArtist;
                    else
                        return artists.FirstOrDefault(artist => artist.name == artistname);
                }
            }
            public Artist this[RawTrack trackinfo]
            {
                get { return this[trackinfo.ArtistName]; }
            }

            public bool Contains(Artist artist)
            {
                return artists.Contains(artist);
            }

            public IEnumerable<Track> GetTracks()
            {
                foreach (Artist a in this)
                    foreach (Track t in a.Albums.GetTracks())
                        yield return t;
            }

            IEnumerator<Artist> IEnumerable<Artist>.GetEnumerator()
            {
                yield return unknownArtist;
                foreach (Artist a in artists)
                    yield return a;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                yield return unknownArtist;
                foreach (Artist a in artists)
                    yield return a;
            }
        }
    }
}
