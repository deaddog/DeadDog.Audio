using System;
namespace DeadDog.Audio.Playback
{
    public interface IPlayback<T> : IDisposable
    {
        bool CanOpen(T element);
        bool Open(T element);
        bool Close();

        bool Play();
        bool Pause();
        bool Stop();

        /// <summary>
        /// Occurs when the value of the Status property is changed.
        /// </summary>
        event EventHandler StatusChanged;
        /// <summary>
        /// Occurs when the value of the Position property is changed;
        /// </summary>
        event PositionChangedEventHandler PositionChanged;

        bool Seek(PlayerSeekOrigin origin, uint offset);

        PlayerStatus Status { get; }
        uint Position { get; }
        uint Length { get; }

        /// <summary>
        /// Gets or sets the left volume in the range [0-1].
        /// </summary>
        double LeftVolume { get; set; }
        /// <summary>
        /// Gets or sets the right volume in the range [0-1].
        /// </summary>
        double RightVolume { get; set; }
    }
}
