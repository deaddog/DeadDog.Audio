using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Album
    {
        public class AlbumCollection : IEnumerable<Album>
        {
            private List<Album> albums;
            private Album unknownAlbum;

            internal AlbumCollection()
            {
                this.unknownAlbum = new Album(null);
                albums = new List<Album>();
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

            public bool Contains(Album album)
            {
                return albums.Contains(album);
            }
            public bool Contains(string albumname)
            {
                return albums.BinarySearch(albumname, (x, y) => x.CompareTo(y), album => album.title) >= 0;
            }

            public event AlbumEventHandler AlbumAdded, AlbumRemoved;

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
        }
    }
}
