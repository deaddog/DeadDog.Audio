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

        private const double MAXVOLUME = 100;

        public AudioControl()
        {
            this.player = new ZPlay();

            this.info = new TStreamInfo();
            this.time = new TStreamTime();
        }

        public bool CanOpen(string filepath)
        {
            return player.GetFileFormat(filepath) != TStreamFormat.sfUnknown;
        }

        public bool Open(string filepath)
        {
            if (!player.OpenFile(filepath, TStreamFormat.sfAutodetect))
                return false;

            player.GetStreamInfo(ref info);
            return true;
        }
        public bool Close() => player.Close();

        public bool StartPlayback() => player.StartPlayback();
        public bool PausePlayback() => player.PausePlayback();
        public bool ResumePlayback() => player.ResumePlayback();
        public bool StopPlayback() => player.StopPlayback();

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

        public void SetVolume(double left, double right)
        {
            player.SetPlayerVolume((int)(left * MAXVOLUME), (int)(right * MAXVOLUME));
        }
        public void GetVolume(out double left, out double right)
        {
            int l = 0, r = 0;
            player.GetPlayerVolume(ref l, ref r);

            left = l / MAXVOLUME;
            right = r / MAXVOLUME;
        }

        public void Dispose()
        {
            this.player = null;
        }
    }
}
