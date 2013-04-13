using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libZPlay;

namespace DeadDog.Audio
{
    /// <summary>
    /// Enables playback of audio-files using the libzplay multimedia library.
    /// </summary>
    /// <remarks>
    /// libzplay is developed by Zoran Cindori, see http://libzplay.sourceforge.net/ for more.
    /// </remarks>
    public class AudioControl : IDisposable
    {
        private string filepath = null;
        private ZPlay player;

        private PlayerStatus plStatus = PlayerStatus.NoFileOpen;

        private TStreamInfo info;
        private TStreamStatus status;
        private TStreamTime time;

        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// Gets a value indicating if a <see cref="System.Windows.Forms.Timer"/> is associated with the <see cref="AudioControl"/> and thus determines the <see cref="Update"/> is invoked automatically.
        /// </summary>
        public bool UsesTimer
        {
            get { return timer != null; }
        }

        public AudioControl(bool usesTimer)
        {
            this.player = new ZPlay();

            this.info = new TStreamInfo();
            this.status = new TStreamStatus();
            this.time = new TStreamTime();

            this.timer = usesTimer ? new System.Windows.Forms.Timer() : null;
            if (this.timer != null)
            {
                this.timer.Interval = 100;
                this.timer.Tick += new EventHandler(timer_Tick);
            }
        }

        #region Events
        /// <summary>
        /// Occurs when the playing has reached it's end.
        /// </summary>
        public event EventHandler PlaybackEnd;
        /// <summary>
        /// Occurs when a small portion of the currently playing file has been played. Is therefore not called when the file is paused.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// Occurs when playback starts.
        /// </summary>
        public event EventHandler PlaybackStart;
        /// <summary>
        /// Occurs when playback is paused.
        /// </summary>
        public event EventHandler Paused;
        /// <summary>
        /// Occurs when playback is resumed (from pause).
        /// </summary>
        public event EventHandler Resumed;
        /// <summary>
        /// Occurs when playback is stopped.
        /// </summary>
        public event EventHandler Stopped;
        #endregion

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

                    if (UsesTimer)
                        timer.Start();
                    return true;
                case PlayerStatus.Stopped:
                    if (!player.StartPlayback())
                        throw new Exception("Player failed to start playback.");
                    plStatus = PlayerStatus.Playing;
                    Update();
                    if (PlaybackStart != null)
                        PlaybackStart(this, EventArgs.Empty);

                    if (UsesTimer)
                        timer.Start();
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

                    if (UsesTimer)
                        timer.Stop();
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

                    if (UsesTimer)
                        timer.Stop();
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

        /// <summary>
        /// Updates the current state of this <see cref="AudioControl"/>.
        /// This method should never be called directly when <see cref="UsesTimer"/> is true.
        /// </summary>
        public void Update()
        {
            player.GetStatus(ref status);
            player.GetPosition(ref time);

            if (plStatus == PlayerStatus.Playing && !status.fPlay
                && Position == 0 && Length > 0)
            {
                plStatus = PlayerStatus.Stopped;
                timer.Stop();

                if (this.Tick != null)
                    Tick(this, EventArgs.Empty);

                if (this.PlaybackEnd != null)
                    PlaybackEnd(this, EventArgs.Empty);
            }
            else if (this.Tick != null)
                Tick(this, EventArgs.Empty);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            this.Update();
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
