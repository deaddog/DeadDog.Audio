using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents a collection of objects that can be iterated through a selection of commands.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPlaylist<T> : IEnumerable<PlaylistEntry<T>>
    {
        /// <summary>
        /// Gets the currently selected <see cref="PlaylistEntry{T}"/>.
        /// </summary>
        /// <value>
        /// The currently selected <see cref="PlaylistEntry{T}"/>.
        /// </value>
        PlaylistEntry<T> CurrentEntry { get; }

        event EntryChangedEventHandler EntryChanged;

        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();

        bool Contains(PlaylistEntry<T> entry);

        /// <summary>
        /// Resets the playlist.
        /// </summary>
        void Reset();
    }
}
