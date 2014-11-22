using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents a collection of objects of which the currently active one is retrieveable through a property.
    /// </summary>
    /// <typeparam name="T">The type of elements in the playlist.</typeparam>
    public interface IPlaylist<T> : IPlayable<T>
    {
        /// <summary>
        /// Resets the playlist.
        /// </summary>
        void Reset();
    }
}
