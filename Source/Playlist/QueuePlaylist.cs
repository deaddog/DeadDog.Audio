using System;
using System.Collections.Generic;

namespace DeadDog.Audio
{
    public class QueuePlaylist<T> : IPlaylist<T>
        where T : class
    {
        private ISeekablePlaylist<T> playlist;
        private IQueue<T> queue;

        public QueuePlaylist(IQueue<T> queue, ISeekablePlaylist<T> fallbackPlaylist)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            if (fallbackPlaylist == null)
                throw new ArgumentNullException("fallbackPlaylist");

            this.queue = queue;
            this.playlist = fallbackPlaylist;

            this.playlist.EntryChanged += playlist_EntryChanged;
            this.playlist.EntryChanging += playlist_EntryChanging;
        }
        public QueuePlaylist(Queue<T> queue, ISeekablePlaylist<T> fallbackPlaylist)
            : this(new QueueInterfaceWrapper<T>(queue), fallbackPlaylist)
        {
        }

        public T Entry
        {
            get { return playlist.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        private void playlist_EntryChanged(object sender, EventArgs e)
        {
            if (EntryChanged != null)
                EntryChanged(this, e);
        }
        private void playlist_EntryChanging(IPlayable<T> sender, EntryChangingEventArgs<T> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
        }

        public bool MoveNext()
        {
            while (queue.Count > 0)
            {
                T item = queue.Dequeue();
                if (item == null)
                    throw new NullReferenceException("The queded element cannot be null.");

                if (!playlist.Contains(item))
                    throw new KeyNotFoundException("The queued element was not found in the playlist.");

                if (TryChangingEntry(item))
                    return playlist.MoveToEntry(item);
            }

            return playlist.MoveNext();
        }
        public bool TryChangingEntry(T entry)
        {
            if (EntryChanging != null)
            {
                EntryChangingEventArgs<T> e = new EntryChangingEventArgs<T>(entry);
                EntryChanging(this, e);
                if (e.Rejected)
                    return false;
            }

            return true;
        }

        public void Reset()
        {
            queue.Clear();
            playlist.Reset();
        }

        private class QueueInterfaceWrapper<T> : IQueue<T>
        {
            private Queue<T> queue;

            public QueueInterfaceWrapper(Queue<T> queue)
            {
                if (queue == null)
                    throw new ArgumentNullException("queue");

                this.queue = queue;
            }

            public static implicit operator QueueInterfaceWrapper<T>(Queue<T> q)
            {
                return new QueueInterfaceWrapper<T>(q);
            }

            public int Count
            {
                get { return queue.Count; }
            }
            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Enqueue(T entry)
            {
                queue.Enqueue(entry);
            }
            public T Dequeue()
            {
                return queue.Dequeue();
            }

            public void Clear()
            {
                queue.Clear();
            }
            public bool Contains(T item)
            {
                return queue.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                queue.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return queue.GetEnumerator();
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return queue.GetEnumerator();
            }
        }
    }
}
