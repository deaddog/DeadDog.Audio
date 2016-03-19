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
    public class AudioControl : IFilePlayback
    {
        private ZPlay player;

        private TStreamInfo info;
        private TStreamTime time;

        private int volumeLeft = 0, volumeRight = 0;

        public AudioControl()
        {
            this.player = new ZPlay();
            readVolumes();

            this.info = new TStreamInfo();
            this.time = new TStreamTime();
        }

        public bool CanOpen(string filepath)
        {
            return player.GetFileFormat(filepath) != TStreamFormat.sfUnknown;
        }

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
                    if (!CanOpen(element))
                        return false;

                    if (!player.OpenFile(System.IO.Path.GetFullPath(getFilename(element)), TStreamFormat.sfAutodetect))
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
            TStreamTime seekTime = new TStreamTime() { ms = offset };
            bool r = player.Seek(TTimeFormat.tfMillisecond, ref seekTime, TranslateSeek(origin));
            return r;

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

        public uint GetTrackLength()
        {
            TStreamInfo info = new TStreamInfo();
            player.GetStreamInfo(ref info);
            return info.Length.ms;
        }
        public uint GetTrackPosition()
        {
            TStreamTime time = new TStreamTime();
            player.GetPosition(ref time);
            return time.ms;
        }
        public bool GetIsPlaying()
        {
            TStreamStatus status = new TStreamStatus();
            player.GetStatus(ref status);
            return status.fPlay;
        }

        public int LeftVolume
        {
            get
            {
                readVolumes();
                return volumeLeft;
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("Volume must be in range [0-100].");
                readVolumes(); player.SetPlayerVolume(value, volumeRight);
            }
        }
        public int RightVolume
        {
            get
            {
                readVolumes();
                return volumeRight;
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("Volume must be in range [0-100].");
                readVolumes(); player.SetPlayerVolume(volumeLeft, value);
            }
        }

        private void readVolumes()
        {
            this.player.GetPlayerVolume(ref volumeLeft, ref volumeRight);
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
