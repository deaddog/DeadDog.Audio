using System;

namespace DeadDog.Audio.Playback
{
    public class Player<T> : IDisposable
        where T : DeadDog.Audio.Libraries.Track
    {
        private IPlaylist<T> playlist;
        private IPlayback playback;

        public Player(IPlaylist<T> playlist, IPlayback playback)
        {
            this.playlist = playlist;
            this.playback = playback;
        }

        public PlayerStatus Status
        {
            get { return playback.Status; }
        }
        public uint Position
        {
            get { return playback.Position; }
            set { Seek(PlayerSeekOrigin.Begin, value); }
        }
        public uint Length
        {
            get { return playback.Length; }
        }
        public double PercentPlayed
        {
            get { return (double)Position / (double)Length; }
        }

        void IDisposable.Dispose()
        {
            Stop();
            playback.Close();
            playback.Dispose();
            playback = null;
            this.Dispose();
        }

        protected void Dispose()
        {
        }
    }
}
