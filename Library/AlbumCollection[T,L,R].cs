using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Album<T, L, R>
    {
        public class AlbumCollection : IEnumerable<L>
        {
            private R artist;
            private L unknownAlbum;
            private List<L> albums;

            internal AlbumCollection(R artist, L unknownAlbum)
            {
                this.artist = artist;

                this.unknownAlbum = unknownAlbum;
                albums = new List<L>();
            }

            public L GetAlbum(string albumtitle)
            {
                if (albumtitle == null || albumtitle.Length == 0)
                    return unknownAlbum;

                for (int i = 0; i < albums.Count; i++)
                    if (albums[i].Equals(albumtitle))
                        return albums[i];

                return null;
            }
            public L GetAlbum(RawTrack trackinfo)
            {
                return GetAlbum(trackinfo.AlbumTitle);
            }

            internal void Remove(L item)
            {
                albums.Remove(item);
            }
            internal bool Contains(L item)
            {
                if (unknownAlbum == item)
                    return true;
                else
                    return albums.Contains(item);
            }

            internal void MergeWith(AlbumCollection collection)
            {
                if (this == collection)
                    return;

                collection.unknownAlbum.Tracks.MergeWith(this.unknownAlbum.Tracks);
                while (collection.albums.Count > 0)
                {
                    L album = collection.albums[0];
                    collection.albums.Remove(album);
                    album.artist = this.artist;

                    int index = albums.BinarySearch(album, album);
                    if (~index < 0)
                        albums.Add(album);
                    else
                        albums.Insert(~index, album);
                }
            }

            internal L CreateAlbum(LibraryFactory<T, L, R> factory, RawTrack trackinfo)
            {
                L album = factory.CreateAlbum();
                album.initialize(trackinfo, artist);

                int index = albums.BinarySearch(album, album);
                if (~index < 0)
                    albums.Add(album);
                else
                    albums.Insert(~index, album);

                return album;
            }
            internal void DisposeAlbum(L item)
            {
                if (!albums.Contains(item))
                    throw new ArgumentException("item is not part of collection", "item");

                albums.Remove(item);
                item.destroy();
            }
            internal void Dispose()
            {
                while (albums.Count > 0)
                {
                    DisposeAlbum(albums[0]);
                }
            }

            public L UnknownAlbum
            {
                get { return unknownAlbum; }
            }

            public int Count
            {
                get { return albums.Count; }
            }
            public L this[int index]
            {
                get { return albums[index]; }
            }

            public override string ToString()
            {
                return "AlbumCollection [" + Count + "]";
            }

            #region IEnumerable<L> Members

            public IEnumerator<L> GetEnumerator()
            {
                return albums.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (albums as System.Collections.IEnumerable).GetEnumerator();
            }

            #endregion

            /*internal Album GetAlbum(RawTrack trackinfo, Library library)
        {
            if (trackinfo.AlbumTitle == null || trackinfo.AlbumTitle.Length == 0)
                return unknownAlbum;

            for (int i = 0; i < albums.Count; i++)
                if (albums[i].Title == trackinfo.AlbumTitle)
                    return albums[i];
            //Not found
            Album a = new Album(trackinfo.AlbumTitle, this);
            int index = albums.BinarySearch(a, library.Sorter);
            if (~index < 0)
                albums.Add(a);
            else
                albums.Insert(~index, a);
            return a;
        }*/
        }
    }
}
