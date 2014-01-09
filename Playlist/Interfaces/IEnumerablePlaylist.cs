using System.Collections.Generic;

namespace DeadDog.Audio
{
    public interface IEnumerablePlaylist<T> : IPlaylist<T>, IEnumerable<T>
    {
        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();
        bool MovePrevious();
        bool MoveToFirst();
        bool MoveToLast();
    }
}
