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
    public interface IPlaylist<T>
    {
        /// <summary>
        /// Gets the currently selected entry in the playlist.
        /// </summary>
        T Entry { get; }
        
        /// <summary>
        /// Occurs after the <see cref="Entry"/> property is changed.
        /// </summary>
        event EventHandler EntryChanged;
        /// <summary>
        /// Occurs before the <see cref="Entry"/> property is changed.
        /// </summary>
        event EntryChangingEventHandler<T> EntryChanging;

        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();

        /// <summary>
        /// Resets the playlist.
        /// </summary>
        void Reset();
    }
}
