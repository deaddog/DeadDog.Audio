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

        public bool Play()
        {
            switch (playback.Status)
            {
                case PlayerStatus.Playing:
                    playback.Seek(PlayerSeekOrigin.Begin, 0);
                    return true;
                case PlayerStatus.Paused:
                    if (!playback.Play())
                        throw new Exception("Player failed to resume playback.");
                    return true;
                case PlayerStatus.Stopped:
                    if (!playback.Play())
                        throw new Exception("Player failed to start playback.");
                    return true;
                case PlayerStatus.NoFileOpen:
                    if (playlist.MoveNext())
                    {
                        if (!playlist.CurrentEntry.Track.FileExist)
                            return Play();
                        playback.Open(playlist.CurrentEntry.Track.FilePath);
                        playback.Play();
                        return true;
                    }
                    else
                        return false;
                default:
                    throw new InvalidOperationException("Unknown playback state: " + playback.Status);
            }
        }
        public bool Pause()
        {
            switch (playback.Status)
            {
                case PlayerStatus.Playing:
                    if (!playback.Pause())
                        throw new Exception("Player failed to pause playback.");
                    return true;
                case PlayerStatus.Paused:
                    return Play();
                case PlayerStatus.Stopped:
                    return false;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new InvalidOperationException("Unknown playback state: " + playback.Status);
            }
        }

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
