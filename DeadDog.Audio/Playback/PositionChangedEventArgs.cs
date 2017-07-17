using System;

namespace DeadDog.Audio.Playback
{
    public class PositionChangedEventArgs : EventArgs
    {
        private bool endreached;

        public PositionChangedEventArgs(bool endreached)
        {
            this.endreached = endreached;
        }

        public bool EndReached
        {
            get { return endreached; }
        }
    }
}
