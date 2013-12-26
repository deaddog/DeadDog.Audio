using System;
namespace DeadDog.Audio.Playback
{
    public interface IPlayback : IDisposable
    {
        public void Open(string filepath);
        public bool Close();

        public bool Play();
        public bool Pause();
        public bool Stop();

        public bool Seek(PlayerSeekOrigin origin, uint offset);

        public PlayerStatus Status { get; }
        public uint Position { get; set; }
        public uint Length { get; }
    }
}
