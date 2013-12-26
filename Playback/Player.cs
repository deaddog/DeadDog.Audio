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

            this.playback.PositionChanged += playback_PositionChanged;
            this.playback.StatusChanged += playback_StatusChanged;
        }

        #region PositionChanged event

        private void playback_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            OnPositionChanged(e);
        }
        protected virtual void OnPositionChanged(PositionChangedEventArgs e)
        {
            if (PositionChanged != null)
                PositionChanged(this, e);
        }
        public event PositionChangedEventHandler PositionChanged;

        #endregion

        #region StatusChanged event

        private void playback_StatusChanged(object sender, EventArgs e)
        {
            OnStatusChanged(e);
        }
        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
        }
        public event EventHandler StatusChanged;

        #endregion

        public PlayerStatus Status
        {
            get { return playback.Status; }
        }
        public uint Position
        {
            get { return playback.Position; }
            set { playback.Seek(PlayerSeekOrigin.Begin, value); }
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
