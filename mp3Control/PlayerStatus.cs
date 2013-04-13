using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Specifies a state for an audio player.
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// The audio player is playing.
        /// </summary>
        Playing,
        /// <summary>
        /// The audio player is paused.
        /// </summary>
        Paused,
        /// <summary>
        /// The audio player is stopped.
        /// </summary>
        Stopped,
        /// <summary>
        /// The audio player has not loaded a file.
        /// </summary>
        NoFileOpen
    }
}
