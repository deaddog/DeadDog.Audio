using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Track<T, L, R>
    {
        public class TrackCollection : IEnumerable<T>
        {
            private L album;
            private List<T> tracks;

            internal TrackCollection(L album)
            {
                this.album = album;
                tracks = new List<T>();
            }

            public T GetTrack(string tracktitle)
            {
                for (int i = 0; i < tracks.Count; i++)
                    if (tracks[i].Equals(tracktitle))
                        return tracks[i];

                return null;
            }
            public T GetTrack(RawTrack trackinfo)
            {
                return GetTrack(trackinfo.TrackTitle);
            }

            internal void Remove(T item)
            {
                tracks.Remove(item);
            }
            internal bool Contains(T item)
            {
                return tracks.Contains(item);
            }

            /// <summary>
            /// Merges a <see cref="TrackCollection"/> into this <see cref="TrackCollection"/>.
            /// </summary>
            /// <param name="collection">The <see cref="TrackCollection"/> to merge into this <see cref="TrackCollection"/></param>
            internal void MergeWith(TrackCollection collection)
            {
                if (this == collection)
                    return;

                while (collection.tracks.Count > 0)
                {
                    T track = collection.tracks[0];
                    collection.tracks.Remove(track);
                    track.album = this.album;

                    int index = tracks.BinarySearch(track, track);
                    if (~index < 0)
                        tracks.Add(track);
                    else
                        tracks.Insert(~index, track);
                }
            }
            private static void mergeAlbums(TrackCollection from, TrackCollection to)
            {
                if (to == from)
                    return;

                while (from.tracks.Count > 0)
                {
                    T track = from.tracks[0];
                    from.tracks.Remove(track);
                    track.album = to.album;
                    to.tracks.Add(track);
                }
            }

            internal T CreateTrack(LibraryFactory<T, L, R> factory, RawTrack trackinfo)
            {
                T track = factory.CreateTrack();
                track.initialize(trackinfo, this.album);

                int index = tracks.BinarySearch(track, track);
                if (~index < 0)
                    tracks.Add(track);
                else
                    tracks.Insert(~index, track);

                return track;
            }
            internal void DisposeTrack(T item)
            {
                if (!tracks.Contains(item))
                    throw new ArgumentException("item is not part of collection", "item");

                tracks.Remove(item);
                item.destroy();
            }
            internal void Dispose()
            {
                while (tracks.Count > 0)
                {
                    DisposeTrack(tracks[0]);
                }
            }

            public int Count
            {
                get { return tracks.Count; }
            }
            public T this[int index]
            {
                get { return tracks[index]; }
            }

            public override string ToString()
            {
                return "TrackCollection [" + Count + "]";
            }

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return tracks.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (tracks as System.Collections.IEnumerable).GetEnumerator();
            }

            #endregion
        }
    }
}
