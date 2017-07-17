using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Playlist
{
    public class QueuePlaylist<T> : DecoratorPlaylist<T>
    {
        private Queue<T> _queue;

        public QueuePlaylist(IPlaylist<T> playlist) : base(playlist)
        {
            _queue = new Queue<T>();
        }

        public void Enqueue(T entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "A null value cannot be queued.");

            _queue.Enqueue(entry);
        }

        public override bool TryPeekNext(out T entry)
        {
            if (_queue.Count > 0)
            {
                entry = _queue.Peek();
                return true;
            }
            else
                return base.TryPeekNext(out entry);
        }

        public override bool MoveNext()
        {
            while (_queue.Count > 0)
            {
                var item = _queue.Dequeue();
                if (base.MoveToEntry(item))
                    return true;
            }

            return base.MoveNext();
        }
        public override void Reset()
        {
            _queue.Clear();
            base.Reset();
        }
    }
}
