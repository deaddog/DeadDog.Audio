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

        /// <summary>
        /// Occurs when the value of the Status property is changed.
        /// </summary>
        public event EventHandler StatusChanged;
        /// <summary>
        /// Occurs when the value of the Position property is changed;
        /// </summary>
        public event PositionChangedEventHandler PositionChanged;

        public bool Seek(PlayerSeekOrigin origin, uint offset);

        public PlayerStatus Status { get; }
        public uint Position { get; }
        public uint Length { get; }
    }
}
