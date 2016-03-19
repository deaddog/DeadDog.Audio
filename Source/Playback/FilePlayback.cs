using System;

namespace DeadDog.Audio.Playback
{
    public class FilePlayback<T> : IPlayback<T>
    {
        private readonly IFilePlayback playback;
        private readonly Func<T, string> getFilename;

        private PlayerStatus status;

        private uint trackLength;
        private uint trackPosition;

        private const int TIMER_INTERVAL = 100;
        private const int TIMER_INFINITE = System.Threading.Timeout.Infinite;
        private System.Threading.Timer timer;

        public FilePlayback(IFilePlayback playback, Func<T, string> getFilename)
        {
            if (playback == null)
                throw new ArgumentNullException(nameof(playback));
            if (getFilename == null)
                throw new ArgumentNullException(nameof(getFilename));

            this.playback = playback;
            this.getFilename = getFilename;

            this.status = PlayerStatus.NoFileOpen;

            this.trackLength = 0;
            this.trackPosition = 0;

            this.timer = new System.Threading.Timer(obj => update(), null, 0, 0);
        }

        public event EventHandler StatusChanged;
        public event PositionChangedEventHandler PositionChanged;

        public bool CanOpen(T element)
        {
            if (element == null)
                return false;

            string fullpath;

            try { fullpath = System.IO.Path.GetFullPath(getFilename(element)); }
            catch { fullpath = null; }
            if (!System.IO.File.Exists(fullpath))
                return false;

            return playback.CanOpen(fullpath);
        }

        public bool Open(T element)
        {
            switch (status)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    Close();
                    return Open(element);

                case PlayerStatus.NoFileOpen:
                    if (!CanOpen(element))
                        return false;

                    if (!playback.Open(System.IO.Path.GetFullPath(getFilename(element))))
                        return false;

                    trackLength = playback.GetTrackLength();
                    Status = PlayerStatus.Stopped;
                    return true;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }
        public bool Close()
        {
            if (status == PlayerStatus.NoFileOpen)
                return true;
            else
            {
                Stop();
                if (!playback.Close())
                    throw new Exception("AudioControl failed to close file.");
                Status = PlayerStatus.NoFileOpen;
                return true;
            }
        }

        public bool Play()
        {
            switch (status)
            {
                case PlayerStatus.Playing:
                    Seek(PlayerSeekOrigin.Begin, 0);
                    return true;
                case PlayerStatus.Paused:
                    if (!playback.ResumePlayback())
                        throw new Exception("Player failed to resume playback.");
                    Status = PlayerStatus.Playing;
                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.Stopped:
                    if (!playback.StartPlayback())
                        throw new Exception("Player failed to start playback.");
                    Status = PlayerStatus.Playing;
                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    return false;
            }
        }
        public bool Pause()
        {
            switch (status)
            {
                case PlayerStatus.Playing:
                    if (!playback.PausePlayback())
                        throw new Exception("Player failed to pause playback.");
                    Status = PlayerStatus.Paused;
                    timer.Change(0, TIMER_INFINITE);
                    return true;
                case PlayerStatus.Paused:
                    return true;
                case PlayerStatus.Stopped:
                    return false;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    return false;
            }
        }
        public bool Stop()
        {
            switch (status)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    if (!playback.StopPlayback())
                        throw new Exception("Player failed to stop playback.");
                    Status = PlayerStatus.Stopped;
                    timer.Change(0, TIMER_INFINITE);
                    return true;
                case PlayerStatus.Stopped:
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    return false;
            }
        }

        public bool Seek(PlayerSeekOrigin origin, uint offset)
        {
            switch (status)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    {
                        bool r = playback.Seek(origin, offset);
                        if (status == PlayerStatus.Paused)
                            update();
                        return r;
                    }
                case PlayerStatus.Stopped:
                    return false;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        public PlayerStatus Status
        {
            get { return status; }
            private set
            {
                status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public uint Length => trackLength;
        public uint Position => trackPosition;

        private void update()
        {
            trackPosition = playback.GetTrackPosition();
            bool isPlaying = playback.GetIsPlaying();

            bool endreached = false;

            if (status == PlayerStatus.Playing && !isPlaying
                && Position == 0 && Length > 0)
            {
                Status = PlayerStatus.Stopped;
                timer.Change(TIMER_INFINITE, TIMER_INFINITE);

                endreached = true;
            }

            PositionChanged?.Invoke(this, new PositionChangedEventArgs(endreached));
        }

        void IDisposable.Dispose()
        {
            Status = PlayerStatus.NoFileOpen;

            playback.Dispose();
        }
    }
}
