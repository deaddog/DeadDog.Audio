using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Album
    {
        public class AlbumCollection : IEnumerable<Album>, ITracks
        {
            private Artist artist;
            private Album unknownAlbum;
            private List<Album> albums;

            internal AlbumCollection(Artist artist, Album unknownAlbum)
            {
                this.artist = artist;

                this.unknownAlbum = unknownAlbum;
                albums = new List<Album>();
            }

            internal Album CreateAlbum(RawTrack trackinfo, AlbumFactory albumFactory)
            {
                Album album = albumFactory.CreateAlbum(trackinfo, this.artist);

                int index = albums.BinarySearch(album, (x, y) => x.title.CompareTo(y.title));
                if (~index < 0)
                    albums.Add(album);
                else
                    albums.Insert(~index, album);

                return album;
            }
            internal void RemoveAlbum(Album album)
            {
                albums.Remove(album);
            }

            public Album UnknownAlbum
            {
                get { return unknownAlbum; }
            }

            public int Count
            {
                get { return albums.Count; }
            }
            public Album this[int index]
            {
                get { return albums[index]; }
            }
            public Album this[string albumtitle]
            {
                get
                {
                    if (albumtitle == null || albumtitle.Length == 0)
                        return unknownAlbum;
                    else
                        return albums.FirstOrDefault(album => album.title == albumtitle);
                }
            }
            public Album this[RawTrack trackinfo]
            {
                get { return this[trackinfo.AlbumTitle]; }
            }

            public bool Contains(Album album)
            {
                return albums.Contains(album);
            }

            IEnumerator<Album> IEnumerable<Album>.GetEnumerator()
            {
                yield return unknownAlbum;
                foreach (Album a in albums)
                    yield return a;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                yield return unknownAlbum;
                foreach (Album a in albums)
                    yield return a;
            }

            public IEnumerable<Track> GetTracks()
            {
                foreach (Album a in this)
                    foreach (Track t in a.Tracks)
                        yield return t;
            }
        }
    }
}
