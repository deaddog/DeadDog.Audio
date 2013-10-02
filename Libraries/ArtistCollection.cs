using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Artist
    {
        public class ArtistCollection : IEnumerable<Artist>
        {
            private List<Artist> artists;
            private Artist unknownArtist;

            internal ArtistCollection()
            {
                this.unknownArtist = new Artist(null);
                this.artists = new List<Artist>();
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

            public event ArtistEventHandler ArtistAdded, ArtistRemoved;

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
