using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public class PlaylistEntry<T>
    {
        private T track;

        public T Track
        {
            get { return track; }
        }

        public PlaylistEntry(T track)
        {
            this.track = track;
        }

        public override string ToString()
        {
            return track.ToString();
        }
    }
}
