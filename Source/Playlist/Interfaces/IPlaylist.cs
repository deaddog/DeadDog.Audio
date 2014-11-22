using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents a collection of objects that can be traversed in multiple ways.
    /// </summary>
    /// <typeparam name="T">The type of elements in the playlist.</typeparam>
    public interface IPlaylist<T> : IPlayable<T>, IEnumerable<T>
    {
        /// <summary>
        /// Moves the the previous item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MovePrevious();
        /// <summary>
        /// Moves the the first item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveToFirst();
        /// <summary>
        /// Moves the the last item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveToLast();

        /// <summary>
        /// Moves to <paramref name="entry"/> if it exists in the playlist.
        /// </summary>
        /// <param name="entry">The entry to move to.</param>
        /// <returns>true, if <paramref name="entry"/> exists in the playlist and the move was successful; otherwise false.</returns>
        bool MoveToEntry(T entry);
        /// <summary>
        /// Determines whether the playlist contains <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to search for in the playlist.</param>
        /// <returns>true, if <paramref name="entry"/> exists in the playlist; otherwise false.</returns>
        bool Contains(T entry);

        /// <summary>
        /// Resets the playlist.
        /// </summary>
        void Reset();
    }
}
