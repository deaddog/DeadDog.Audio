using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Track
    {
        public class TrackCollection : LibraryCollectionBase<Track>
        {
            internal TrackCollection()
            {
            }

            internal override Track _unknownElement
            {
                get { return null; }
            }

            public event TrackEventHandler TrackAdded, TrackRemoved;
            protected override void OnAdded(Track element)
            {
                if (TrackAdded != null)
                    TrackAdded(this, new TrackEventArgs(element));
            }
            protected override void OnRemoved(Track element)
            {
                if (TrackRemoved != null)
                    TrackRemoved(this, new TrackEventArgs(element));
            }

            protected override string GetName(Track element)
            {
                return element.title;
            }
            protected override int Compare(Track element1, Track element2)
            {
                int? v1 = element1.tracknumber, v2 = element2.tracknumber;
                if (v1.HasValue)
                    return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
                else
                    return v2.HasValue ? -1 : 0;
            }
        }
    }
}
