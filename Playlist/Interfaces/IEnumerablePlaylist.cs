using System.Collections.Generic;

namespace DeadDog.Audio
{
    public interface IEnumerablePlaylist<T> : IPlaylist<T>, IEnumerable<T>
    {
        bool MovePrevious();
        bool MoveToFirst();
        bool MoveToLast();
    }
}
