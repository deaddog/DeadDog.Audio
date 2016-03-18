using System;

namespace DeadDog.Audio.Playback
{
    public interface IFilePlayback : IDisposable
    {
        uint GetTrackLength();
        uint GetTrackPosition();
    }
}
