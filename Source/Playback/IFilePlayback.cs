using System;

namespace DeadDog.Audio.Playback
{
    public interface IFilePlayback : IDisposable
    {
        bool CanOpen(string filepath);

        bool Seek(PlayerSeekOrigin origin, uint offset);

        uint GetTrackLength();
        uint GetTrackPosition();
        bool GetIsPlaying();
    }
}
