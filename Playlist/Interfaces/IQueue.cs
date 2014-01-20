using System.Collections.Generic;

namespace DeadDog.Audio
{
    public interface IQueue<T> : IEnumerable<T>
    {
        int Count { get; }
        bool IsReadOnly { get; }

        void Enqueue(T entry);
        T Dequeue();

        void Clear();

        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
    }
}
