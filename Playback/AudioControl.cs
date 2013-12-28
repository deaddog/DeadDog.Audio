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

        public AudioControl()
        {
            this.player = new ZPlay();

            this.info = new TStreamInfo();
            this.status = new TStreamStatus();
            this.time = new TStreamTime();

            this.timer = new System.Threading.Timer(obj => Update(), null, 0, 0);
        }
        
        public event EventHandler StatusChanged;
        public event PositionChangedEventHandler PositionChanged;

        public bool Play()
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                    Seek(PlayerSeekOrigin.Begin, 0);
                    if (PlaybackStart != null)
                        PlaybackStart(this, EventArgs.Empty);
                    return true;
                case PlayerStatus.Paused:
                    if (!player.ResumePlayback())
                        throw new Exception("Player failed to resume playback.");
                    plStatus = PlayerStatus.Playing;
                    Update();
                    if (Resumed != null)
                        Resumed(this, EventArgs.Empty);

                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.Stopped:
                    if (!player.StartPlayback())
                        throw new Exception("Player failed to start playback.");
                    plStatus = PlayerStatus.Playing;
                    Update();
                    if (PlaybackStart != null)
                        PlaybackStart(this, EventArgs.Empty);

                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    return false;
            }
        }
        public bool Play(string filepath)
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    Stop();
                    if (!player.Close())
                        throw new Exception("Player failed to close file.");
                    plStatus = PlayerStatus.NoFileOpen;
                    return Play(filepath);

                case PlayerStatus.NoFileOpen:
                    if (!player.OpenFile(filepath, TStreamFormat.sfAutodetect))
                        throw new Exception("Player failed to open file.");
                    else
                    {
                        player.GetStreamInfo(ref info);
                        plStatus = PlayerStatus.Stopped;
                        return Play();
                    }

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
                    plStatus = PlayerStatus.Paused;
                    Update();
                    if (Paused != null)
                        Paused(this, EventArgs.Empty);

                    timer.Change(0, TIMER_INFINITE);
                    return true;
                case PlayerStatus.Paused:
                    return Play();
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
                    plStatus = PlayerStatus.Stopped;
                    Update();
                    if (Stopped != null)
                        Stopped(this, EventArgs.Empty);

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
                        Update();
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
        }
        public uint Position
        {
            get { return time.ms; }
            set { Seek(PlayerSeekOrigin.Begin, value); }
        }
        public uint Length
        {
            get { return info.Length.ms; }
        }
        public double PercentPlayed
        {
            get { return (double)Position / (double)Length; }
        }

        private void Update()
        {
            player.GetStatus(ref status);
            player.GetPosition(ref time);

            if (plStatus == PlayerStatus.Playing && !status.fPlay
                && Position == 0 && Length > 0)
            {
                plStatus = PlayerStatus.Stopped;
                timer.Change(TIMER_INFINITE, TIMER_INFINITE);

                if (this.Tick != null)
                    Tick(this, EventArgs.Empty);

                if (this.PlaybackEnd != null)
                    PlaybackEnd(this, EventArgs.Empty);
            }
            else if (this.Tick != null)
                Tick(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Stop();
            player.Close();
            plStatus = PlayerStatus.NoFileOpen;
            this.player = null;
        }
    }
}
