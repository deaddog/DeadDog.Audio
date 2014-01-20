using System;
using System.Collections.Generic;

namespace DeadDog.Audio
{
    public class QueuePlaylist<T> : IPlaylist<T>
        where T : class
    {
        private ISeekablePlaylist<T> playlist;
        private IQueue<T> queue;
        private QueueCompare comparer = new QueueCompare();

        public QueuePlaylist(IQueue<T> queue, ISeekablePlaylist<T> fallbackPlaylist)
        {
            this.queue = queue;
            this.playlist = fallbackPlaylist;
        }

        public T Entry
        {
            get { return playlist.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public bool MoveNext();

        public void Reset()
        {
            queue.Clear();
        }

        public virtual void Enqueue(PlaylistEntry<T> entry)
        {
            QueueEntry<T> e = new QueueEntry<T>(entry);
            int index = queue.BinarySearch(e, comparer);
            queue.Insert(~index, e);
        }

        public virtual PlaylistEntry<T> Dequeue()
        {
            return queue.RemoveAt(0);
        }

        public PlaylistEntry<T> Peek()
        {
            return queue[0].Entry;
        }

        public QueueEntry<T> this[int index]
        {
            get { return queue[index]; }
        }

        public virtual void RemoveAt(int index)
        {
            queue.RemoveAt(index);
        }
        public virtual bool Remove(PlaylistEntry<T> item)
        {
            int c = 0;
            for (int i = 0; i - c < queue.Count; i++)
            {
                if (queue[i].Entry == item)
                {
                    queue.RemoveAt(i - c);
                    c++;
                }
            }
            return c > 0;
        }

        public bool Contains(PlaylistEntry<T> item)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].Entry == item)
                    return true;
            }
            return false;
        }

        public virtual void Clear()
        {
            queue.Clear();
            lastDequeued = null;
        }

        public int Count
        {
            get { return queue.Count; }
        }

        private class QueueCompare : IComparer<QueueEntry<T>>
        {
            public int Compare(QueueEntry<T> x, QueueEntry<T> y)
            {
                return x.CompareTo(y);
            }
        }
    }

    public class PlayQueue<T, Q> : IPlayQueue<T>
    {
        private QueueFactory<T, Q> factory;
        private List<QueueEntry<T, Q>> queue;
        private QueueEntry<T, Q> lastDequeued = null;
        private QueueCompare comparer = new QueueCompare();

        public virtual void Enqueue(PlaylistEntry<T> entry)
        {
            QueueEntry<T, Q> e = factory.Construct(entry);
            int index = queue.BinarySearch(e, comparer);
            queue.Insert(~index, e);
        }

        public virtual void Enqueue(PlaylistEntry<T> entry, Q queueInfo)
        {
            QueueEntry<T, Q> e = factory.Construct(entry, queueInfo);
            int index = queue.BinarySearch(e, comparer);
            queue.Insert(~index, e);
        }

        public virtual PlaylistEntry<T> Dequeue()
        {
            lastDequeued = queue[0];
            queue.RemoveAt(0);
            return lastDequeued.Entry;
        }

        public PlaylistEntry<T> Peek()
        {
            return queue[0].Entry;
        }

        public QueueEntry<T, Q> this[int index]
        {
            get { return queue[index]; }
        }

        public virtual void RemoveAt(int index)
        {
            queue.RemoveAt(index);
        }
        public virtual bool Remove(PlaylistEntry<T> item)
        {
            int c = 0;
            for (int i = 0; i - c < queue.Count; i++)
            {
                if (queue[i].Entry == item)
                {
                    queue.RemoveAt(i - c);
                    c++;
                }
            }
            return c > 0;
        }

        public bool Contains(PlaylistEntry<T> item)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].Entry == item)
                    return true;
            }
            return false;
        }

        public virtual void Clear()
        {
            queue.Clear();
            lastDequeued = null;
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public PlayQueue(QueueFactory<T, Q> factory)
        {
            this.factory = factory;
            this.queue = new List<QueueEntry<T, Q>>();
        }

        private class QueueCompare : IComparer<QueueEntry<T, Q>>
        {
            public int Compare(QueueEntry<T, Q> x, QueueEntry<T, Q> y)
            {
                return x.CompareTo(y);
            }
        }
    }

}
