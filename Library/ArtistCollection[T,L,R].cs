using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Artist<T, L, R>
    {
        public class ArtistCollection : IEnumerable<R>
        {
            private Library<T, L, R> library;

            private R unknownArtist;
            private List<R> artists;

            private Dictionary<string, string> synonyms = new Dictionary<string, string>();
            private Dictionary<string, string> renames = new Dictionary<string, string>();

            internal ArtistCollection(Library<T, L, R> library, R unknownArtist)
            {
                this.library = library;

                this.unknownArtist = unknownArtist;
                artists = new List<R>();
            }

            public R GetArtist(string artistname, bool useSynonyms)
            {
                if (artistname == null || artistname.Length == 0)
                    return unknownArtist;

                if (useSynonyms)
                    artistname = synonymLookUp(artistname);

                for (int i = 0; i < artists.Count; i++)
                    if (artists[i].Equals(artistname))
                        return artists[i];

                return null;
            }
            public R GetArtist(string artistname)
            {
                return GetArtist(artistname, true);
            }
            public R GetArtist(RawTrack trackinfo)
            {
                return GetArtist(trackinfo.ArtistName, true);
            }

            public bool AddRename(string from, string to)
            {
                return false;
                /*
                if (from == null || from.Length == 0)
                    return false;

                if (from == to)
                    return false;

                if (renameExists(from, to))
                    return true;

                if(renames.ContainsKey(from))*/
            }
            private string renameLookUp(string input)
            {
                if (renames.ContainsKey(input))
                    return renameLookUp(renames[input]);
                else
                    return input;
            }
            private bool renameExists(string from, string to)
            {
                return renames.ContainsKey(from) && renames[from] == to;
            }
            public bool AddSynonym(string from, string to)
            {
                if (from == null || from.Length == 0)
                    return false;

                if (from == to)
                    return false;

                if (synonymExists(from, to))
                    return true;

                if (synonyms.ContainsKey(from))
                    synonyms[from] = to;
                else if (synonyms.ContainsKey(to) && synonyms[to] == from)
                    synonyms.Remove(to);
                else
                    synonyms.Add(from, to);

                R fArtist = GetArtist(from, false);
                R tArtist = GetArtist(to, false);

                if (fArtist == null)
                    return true;

                if (tArtist == null)
                {
                    RawTrack rt = (RawTrack)RawTrack.Unknown.Clone();
                    RawTrack.SetNewArtistName(rt, to);

                    tArtist = CreateArtist(library.Factory, rt);
                }

                fArtist.Albums.MergeWith(tArtist.Albums);
                fArtist.destroy();
                artists.Remove(fArtist);

                return true;
            }
            private string synonymLookUp(string input)
            {
                if (synonyms.ContainsKey(input))
                    return synonymLookUp(synonyms[input]);
                else
                    return input;
            }
            private bool synonymExists(string from, string to)
            {
                return synonyms.ContainsKey(from) && synonyms[from] == to;
            }

            internal void Remove(R item)
            {
                artists.Remove(item);
            }
            internal bool Contains(R item)
            {
                if (unknownArtist == item)
                    return true;
                else
                    return artists.Contains(item);
            }

            internal R CreateArtist(LibraryFactory<T, L, R> factory, RawTrack trackinfo)
            {
                RawTrack rt = (RawTrack)trackinfo.Clone();
                RawTrack.SetNewArtistName(rt, synonymLookUp(trackinfo.ArtistName));

                R artist = factory.CreateArtist();
                artist.initialize(rt, library);

                int index = artists.BinarySearch(artist, artist);
                if (~index < 0)
                    artists.Add(artist);
                else
                    artists.Insert(~index, artist);

                return artist;
            }

            public R UnknownArtist
            {
                get { return unknownArtist; }
            }

            public int Count
            {
                get { return artists.Count; }
            }
            public R this[int index]
            {
                get { return artists[index]; }
            }

            public override string ToString()
            {
                return "ArtistCollection [" + Count + "]";
            }

            #region IEnumerable<R> Members

            public IEnumerator<R> GetEnumerator()
            {
                return artists.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (artists as System.Collections.IEnumerable).GetEnumerator();
            }

            #endregion
        }
    }
}
