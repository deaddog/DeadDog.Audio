﻿using DeadDog.Audio.Playlist;
using System;

namespace DeadDog.Audio.Playback
{
    public class Player<T> : IDisposable
        where T : class
    {
        private IPlayable<T> playlist;
        private IPlayback<T> playback;

        public Player(IPlayable<T> playlist, IPlayback<T> playback)
        {
            this.playlist = playlist;
            this.playback = playback;

            this.playlist.EntryChanging += playlist_EntryChanging;
            this.playlist.EntryChanged += playlist_EntryChanged;

            this.playback.PositionChanged += playback_PositionChanged;
            this.playback.StatusChanged += playback_StatusChanged;
        }

        #region PositionChanged event

        private void playback_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            OnPositionChanged(e);
            if (e.EndReached)
            {
                if (playlist.MoveNext())
                    Play();
            }
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

        #region TrackChanged event

        private void playlist_EntryChanged(object sender, EventArgs e)
        {
            PlayerStatus status = playback.Status;

            if (status != PlayerStatus.NoFileOpen)
            {
                Stop();
                playback.Close();
            }

            if (playback.Open(playlist.Entry))
                switch (status)
                {
                    case PlayerStatus.Playing:
                        playback.Play();
                        break;
                    case PlayerStatus.Paused:
                        playback.Play();
                        playback.Pause();
                        break;
                }

            OnTrackChanged(EventArgs.Empty);
        }

        private void playlist_EntryChanging(IPlayable<T> sender, EntryChangingEventArgs<T> e)
        {
            if (!playback.CanOpen(e.NewEntry))
                e.RejectChange();
        }
        protected virtual void OnTrackChanged(EventArgs e)
        {
            if (TrackChanged != null)
                TrackChanged(this, e);
        }
        public event EventHandler TrackChanged;

        #endregion

        public T Track
        {
            get { return playlist.Entry; }
        }

        public bool Play()
        {
            switch (playback.Status)
            {
                case PlayerStatus.Playing:
                    playback.Seek(TimeSpan.Zero);
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
        public bool Stop()
        {
            switch (playback.Status)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    if (!playback.Stop())
                        throw new Exception("Player failed to stop playback.");
                    return true;
                case PlayerStatus.Stopped:
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new InvalidOperationException("Unknown playback state: " + playback.Status);
            }
        }

        public bool Seek(TimeSpan position)
        {
            return playback.Seek(position);
        }

        public PlayerStatus Status
        {
            get { return playback.Status; }
        }
        public TimeSpan Position
        {
            get { return playback.Position; }
            set { playback.Seek(value); }
        }
        public TimeSpan Length
        {
            get { return playback.Length; }
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
