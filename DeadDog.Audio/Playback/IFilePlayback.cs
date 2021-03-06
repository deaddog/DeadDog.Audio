﻿using System;

namespace DeadDog.Audio.Playback
{
    public interface IFilePlayback : IDisposable
    {
        bool CanOpen(string filepath);

        bool Open(string filepath);
        bool Close();

        bool StartPlayback();
        bool PausePlayback();
        bool ResumePlayback();
        bool StopPlayback();

        bool Seek(TimeSpan offset);

        TimeSpan GetTrackLength();
        TimeSpan GetTrackPosition();
        bool GetIsPlaying();

        void SetVolume(double left, double right);
        void GetVolume(out double left, out double right);
    }
}
