using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public interface IPlayQueue<T>
    {
        void Enqueue(PlaylistEntry<T> entry);
        PlaylistEntry<T> Dequeue();

        bool Remove(PlaylistEntry<T> item);

        void Clear();

        int Count
        {
            get;
        }
    }
}
