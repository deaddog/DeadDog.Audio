using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public interface IQueuePeekable<T> : IPlayQueue<T>
    {
        PlaylistEntry<T> Peek();
    }
}
