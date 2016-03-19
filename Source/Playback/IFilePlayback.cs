using System;

namespace DeadDog.Audio.Playback
{
    public interface IFilePlayback : IDisposable
    {
        bool CanOpen(string filepath);

        bool StartPlayback();
        bool PausePlayback();
        bool ResumePlayback();
        bool StopPlayback();

        bool Seek(PlayerSeekOrigin origin, uint offset);

        uint GetTrackLength();
        uint GetTrackPosition();
        bool GetIsPlaying();
    }
}
