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

        private const double MAXVOLUME = 100;

        public AudioControl()
        {
            this.player = new ZPlay();
        }

        public bool CanOpen(string filepath)
        {
            return player.GetFileFormat(filepath) != TStreamFormat.sfUnknown;
        }

        public bool Open(string filepath) => player.OpenFile(filepath, TStreamFormat.sfAutodetect);
        public bool Close() => player.Close();

        public bool StartPlayback() => player.StartPlayback();
        public bool PausePlayback() => player.PausePlayback();
        public bool ResumePlayback() => player.ResumePlayback();
        public bool StopPlayback() => player.StopPlayback();

        public bool Seek(TimeSpan position)
        {
            TStreamTime seekTime = new TStreamTime() { ms = (uint)position.TotalMilliseconds };
            bool r = player.Seek(TTimeFormat.tfMillisecond, ref seekTime, TSeekMethod.smFromBeginning);
            return r;

        }

        public TimeSpan GetTrackLength()
        {
            TStreamInfo info = new TStreamInfo();
            player.GetStreamInfo(ref info);
            return TimeSpan.FromMilliseconds(info.Length.ms);
        }
        public TimeSpan GetTrackPosition()
        {
            TStreamTime time = new TStreamTime();
            player.GetPosition(ref time);
            return TimeSpan.FromMilliseconds(time.ms);
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
