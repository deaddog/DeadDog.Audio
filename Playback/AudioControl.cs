using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libZPlay;

namespace DeadDog.Audio.Playback
{
    /// <summary>
    /// Enables playback of audio-files using the libzplay multimedia library.
    /// </summary>
    /// <remarks>
    /// libzplay is developed by Zoran Cindori, see http://libzplay.sourceforge.net/ for more.
    /// </remarks>
    public class AudioControl<T> : IPlayback<T>
        where T : class
    {
        private ZPlay player;

        private PlayerStatus plStatus = PlayerStatus.NoFileOpen;

        private TStreamInfo info;
        private TStreamStatus status;
        private TStreamTime time;

        private int TIMER_INTERVAL = 100;
        private int TIMER_INFINITE = System.Threading.Timeout.Infinite;
        private System.Threading.Timer timer;

        private Func<T, string> getFilename;

        public AudioControl(Func<T, string> getFilename)
        {
            if (getFilename == null)
                throw new ArgumentNullException("getFilename");
            this.getFilename = getFilename;

            this.player = new ZPlay();

            this.info = new TStreamInfo();
            this.status = new TStreamStatus();
            this.time = new TStreamTime();

            this.timer = new System.Threading.Timer(obj => update(), null, 0, 0);
        }

        public event EventHandler StatusChanged;
        public event PositionChangedEventHandler PositionChanged;

        public bool Open(T element)
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    Close();
                    return Open(element);

                case PlayerStatus.NoFileOpen:
                    string path = getFilename(element);
                    if (path == null)
                        return false;
                    System.IO.FileInfo file = new System.IO.FileInfo(path);

                    if (!file.Exists)
                        return false;
                    if (!player.OpenFile(file.FullName, TStreamFormat.sfAutodetect))
                        return false;

                    player.GetStreamInfo(ref info);
                    Status = PlayerStatus.Stopped;
                    return true;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }
        public bool Close()
        {
            if (plStatus == PlayerStatus.NoFileOpen)
                return true;
            else
            {
                Stop();
                if (!player.Close())
                    throw new Exception("AudioControl failed to close file.");
                Status = PlayerStatus.NoFileOpen;
                return true;
            }
        }

        public bool Play()
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                    Seek(PlayerSeekOrigin.Begin, 0);
                    return true;
                case PlayerStatus.Paused:
                    if (!player.ResumePlayback())
                        throw new Exception("Player failed to resume playback.");
                    Status = PlayerStatus.Playing;
                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.Stopped:
                    if (!player.StartPlayback())
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
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                    if (!player.PausePlayback())
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
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    if (!player.StopPlayback())
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
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    {
                        TStreamTime seekTime = new TStreamTime() { ms = offset };
                        bool r = player.Seek(TTimeFormat.tfMillisecond, ref seekTime, TranslateSeek(origin));
                        if (plStatus == PlayerStatus.Paused)
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
        private TSeekMethod TranslateSeek(PlayerSeekOrigin s)
        {
            switch (s)
            {
                case PlayerSeekOrigin.Begin: return TSeekMethod.smFromBeginning;
                case PlayerSeekOrigin.CurrentForwards: return TSeekMethod.smFromCurrentForward;
                case PlayerSeekOrigin.CurrentBackwards: return TSeekMethod.smFromCurrentBackward;
                case PlayerSeekOrigin.End: return TSeekMethod.smFromEnd;
                default:
                    return default(TSeekMethod);
            }
        }

        public PlayerStatus Status
        {
            get { return plStatus; }
            private set
            {
                plStatus = value;
                if (StatusChanged != null)
                    StatusChanged(this, EventArgs.Empty);
            }
        }
        public uint Position
        {
            get { return time.ms; }
        }
        public uint Length
        {
            get { return info.Length.ms; }
        }

        private void update()
        {
            player.GetStatus(ref status);
            player.GetPosition(ref time);

            bool endreached = false;

            if (plStatus == PlayerStatus.Playing && !status.fPlay
                && Position == 0 && Length > 0)
            {
                Status = PlayerStatus.Stopped;
                timer.Change(TIMER_INFINITE, TIMER_INFINITE);

                endreached = true;
            }

            if (PositionChanged != null)
                PositionChanged(this, new PositionChangedEventArgs(endreached));
        }

        public void Dispose()
        {
            Stop();
            player.Close();
            Status = PlayerStatus.NoFileOpen;
            this.player = null;
        }
    }
}
