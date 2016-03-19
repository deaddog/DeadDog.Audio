using System;

namespace DeadDog.Audio.Playback
{
    public interface IFilePlayback : IDisposable
    {
        bool CanOpen(string filepath);

        uint GetTrackLength();
        uint GetTrackPosition();
    }
}
