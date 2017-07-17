using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Playlist
{
    /// <summary>
    /// Provides a collection of playlists, joining them into one.
    /// </summary>
    /// <typeparam name="T">The type of elements in the playlists.</typeparam>
    public class PlaylistCollection<T> : IPeekablePlaylist<T>, IPlaylist<T>
        where T : class
    {
        private Playlist<IPlaylist<T>> playlists;

        public int Index
        {
            get { return playlists.Index; }
        }

        public PlaylistCollection()
        {
            this.playlists = new Playlist<IPlaylist<T>>();
        }

        public T Entry
        {
            get { return playlists.Entry == null ? null : playlists.Entry.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        private void entryChanged(object sender, EventArgs e)
        {
            if (sender == playlists.Entry)
                if (this.Entry != null)
                    if (EntryChanged != null)
                        EntryChanged(this, e);
        }
        private void entryChanging(IPlayable<T> sender, EntryChangingEventArgs<T> e)
        {
            if (sender == playlists.Entry && EntryChanging != null)
                EntryChanging(this, e);
        }

        public void Reset()
        {
            playlists.Reset();
        }

        public bool TryPeekNext(out T entry)
        {
            var current = playlists.Entry;
            var hasNext = playlists.TryPeekNext(out var next);

            if (current == null)
            {
                if (hasNext && next is IPeekablePlaylist<T> peek && peek.TryPeekNext(out entry))
                    return true;

                entry = default(T);
                return false;
            }
            else
            {
                if (current is IPeekablePlaylist<T> peek && peek.TryPeekNext(out entry))
                    return true;

                entry = default(T);
                return false;
            }
        }

        public bool MoveNext()
        {
            bool startsAsNull = this.Entry == null;

            if (playlists.Entry != null && playlists.Entry.MoveNext())
                return true;
            else if (playlists.MoveNext())
            {
                playlists.Entry.Reset();
                if (playlists.Entry.MoveToFirst())
                    return true;
                else
                    return MoveNext();
            }
            else
            {
                if (!startsAsNull && EntryChanged != null)
                    EntryChanged(this, EventArgs.Empty);
                return false;
            }
        }
        public bool MovePrevious()
        {
            bool startsAsNull = this.Entry == null;

            if (playlists.Entry != null && playlists.Entry.MovePrevious())
                return true;
            else if (playlists.MovePrevious())
            {
                playlists.Entry.Reset();
                if (playlists.Entry.MoveToLast())
                    return true;
                else
                    return MovePrevious();
            }
            else
            {
                if (!startsAsNull && EntryChanged != null)
                    EntryChanged(this, EventArgs.Empty);
                return false;
            }
        }

        public bool MoveToFirst()
        {
            if (playlists.MoveToFirst())
            {
                playlists.Entry.Reset();
                return playlists.Entry.MoveToFirst();
            }
            else
                return false;
        }
        public bool MoveToLast()
        {
            if (playlists.MoveToLast())
            {
                playlists.Entry.Reset();
                return playlists.Entry.MoveToLast();
            }
            else
                return false;
        }

        public bool MoveToEntry(T entry)
        {
            foreach (var p in playlists)
                if (p.Contains(entry))
                {
                    if (playlists.MoveToEntry(p))
                    {
                        playlists.Entry.Reset();
                        return p.MoveToEntry(entry);
                    }
                    else
                        return false;
                }
            return false;
        }
        public bool Contains(T entry)
        {
            foreach (var p in playlists)
                if (p.Contains(entry))
                    return true;
            return false;
        }

        public int Count
        {
            get { return playlists.Count; }
        }

        public IPlaylist<T> this[int index]
        {
            get { return playlists[index]; }
            set { playlists[index] = value; }
        }

        public bool Contains(IPlaylist<T> playlist)
        {
            return playlists.Contains(playlist);
        }
        public bool Move(IPlaylist<T> playlist, int index)
        {
            throw new NotImplementedException();
        }

        public void Add(IPlaylist<T> playlist)
        {
            Insert(playlists.Count, playlist);
        }
        public void Insert(int index, IPlaylist<T> playlist)
        {
            playlists.Insert(index, playlist);

            playlist.EntryChanged += entryChanged;
            playlist.EntryChanging += EntryChanging;
        }
        public bool Remove(IPlaylist<T> playlist)
        {
            if (playlist == null)
                throw new ArgumentNullException("item");

            int index = playlists.IndexOf(playlist);
            if (index == -1)
                return false;
            else
            {
                this.RemoveAt(index);
                return true;
            }
        }
        public void RemoveAt(int index)
        {
            var pl = playlists[index];
            playlists.RemoveAt(index);

            pl.EntryChanged -= entryChanged;
            pl.EntryChanging -= EntryChanging;
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (IPlaylist<T> playlist in playlists)
                foreach (T t in playlist)
                    yield return t;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var t in (this as IEnumerable<T>))
                yield return t;
        }

        #endregion
    }
}
