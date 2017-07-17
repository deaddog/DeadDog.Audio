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
        /// Sets the volume of both the left and right channel in the range [0-1].
        /// </summary>
        /// <param name="left">The left volume.</param>
        /// <param name="right">The right volume.</param>
        void SetVolume(double left, double right);
        /// <summary>
        /// Gets the volume of both the left and the right channel in range [0-1].
        /// </summary>
        /// <param name="left">When the function returns; contains the volume of the left channel.</param>
        /// <param name="right">When the function returns; contains the volume of the right channel.</param>
        void GetVolume(out double left, out double right);
    }
}
