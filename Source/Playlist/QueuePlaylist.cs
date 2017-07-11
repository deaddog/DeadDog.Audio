using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Playlist
{
    public class QueuePlaylist<T> : IPlayable<T>
        where T : class
    {
        private IPlaylist<T> playlist;
        private Queue<T> queue;

        public QueuePlaylist(Queue<T> queue, IPlaylist<T> fallbackPlaylist)
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
    }
}
