﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Track
    {
        public class TrackCollection : IEnumerable<Track>
        {
            private List<Track> tracks;

            internal TrackCollection()
            {
                this.tracks = new List<Track>();
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

            public bool Contains(Track track)
            {
                return tracks.Contains(track);
            }
            public bool Contains(string tracktitle)
            {
                return tracks.BinarySearch(tracktitle, (x, y) => x.CompareTo(y), track => track.title) >= 0;
            }

            public event TrackEventHandler TrackAdded, TrackRemoved;

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
