using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Track
    {
        public class TrackCollection : IEnumerable<Track>
        {
            private Album album;
            private List<Track> tracks;

            internal TrackCollection(Album album)
            {
                this.album = album;
                this.tracks = new List<Track>();
            }

            /// <summary>
            /// Creates a new <see cref="Track"/>, adds it to the collection and returns it.
            /// </summary>
            /// <param name="trackinfo">The <see cref="RawTrack"/> info from which the new track is created.</param>
            /// <param name="factory">The <see cref="TrackFactory"/> used for construction.</param>
            /// <returns>The new <see cref="Track"/> that has been added to the collection.</returns>
            internal Track CreateTrack(RawTrack trackinfo, TrackFactory factory)
            {
                Track track = factory.CreateTrack(trackinfo, this.album);

                int index = tracks.BinarySearch(track, (x, y) => x.tracknumber.CompareTo(y.tracknumber));
                if (~index < 0)
                    tracks.Add(track);
                else
                    tracks.Insert(~index, track);

                return track;
            }
            internal void RemoveTrack(Track track)
            {
                tracks.Remove(track);
            }

            public int Count
            {
                get { return tracks.Count; }
            }
            public Track this[int index]
            {
                get { return tracks[index]; }
            }
            public Track this[string tracktitle]
            {
                get { return tracks.FirstOrDefault(track => track.title == tracktitle); }
            }
            public Track this[RawTrack trackinfo]
            {
                get { return this[trackinfo.TrackTitle]; }
            }

            public bool Contains(Track track)
            {
                return tracks.Contains(track);
            }

            IEnumerator<Track> IEnumerable<Track>.GetEnumerator()
            {
                return tracks.GetEnumerator();
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return tracks.GetEnumerator();
            }
        }
    }
}
