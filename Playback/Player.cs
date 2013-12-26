using System;

namespace DeadDog.Audio.Playback
{
    public class Player<T> where T : DeadDog.Audio.Libraries.Track
    {
        private IPlaylist<T> playlist;
        private IPlayback playback;
    }
}
