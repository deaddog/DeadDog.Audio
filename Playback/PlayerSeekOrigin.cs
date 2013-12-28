using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Playback
{
    /// <summary>
    /// Provides the fields that represent reference points for seeking.
    /// </summary>
    public enum PlayerSeekOrigin
    {
        /// <summary>
        /// Specifies the beginning.
        /// </summary>
        Begin = 0,
        /// <summary>
        /// Specifies the current position.
        /// </summary>
        CurrentForwards = 1,
        /// <summary>
        /// Specifies the current position.
        /// </summary>
        CurrentBackwards = 2,
        /// <summary>
        /// Specifies the ending.
        /// </summary>
        End = 3,
    }
}
