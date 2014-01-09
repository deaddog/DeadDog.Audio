using System.Collections.Generic;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents a collection of objects that can be iterated back and forth through a selection of commands.
    /// </summary>
    /// <typeparam name="T">The type of elements in the playlist.</typeparam>
    public interface IEnumerablePlaylist<T> : IPlaylist<T>, IEnumerable<T>
    {
        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();
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
    }
}
