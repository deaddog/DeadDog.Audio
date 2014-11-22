using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    /// <summary>
    /// Provides a collection of playlists, joining them into one.
    /// </summary>
    /// <typeparam name="T">The type of elements in the playlists.</typeparam>
    public class PlaylistCollection<T> : IPlaylist<T>
        where T : class
    {
        private Playlist<IPlaylist<T>> playlists;

        public int Index
        {
            get { return playlists.Index; }
            set { throw new NotImplementedException(); }
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

        public void Reset()
        {
            playlists.Reset();
        }

        public bool MoveNext()
        {
            if (playlists.Entry != null && playlists.Entry.MoveNext())
                return true;
            else if (playlists.MoveNext())
            {
                if (playlists.Entry.MoveToFirst())
                    return true;
                else
                    return MoveNext();
            }
            else
                return false;
        }
        public bool MovePrevious()
        {
            if (playlists.Entry != null && playlists.Entry.MovePrevious())
                return true;
            else if (playlists.MovePrevious())
            {
                if (playlists.Entry.MoveToLast())
                    return true;
                else
                    return MovePrevious();
            }
            else
                return false;
        }

        public bool MoveToFirst()
        {
            if (playlists.MoveToFirst())
                return playlists.Entry.MoveToFirst();
            else
                return false;
        }
        public bool MoveToLast()
        {
            if (playlists.MoveToLast())
                return playlists.Entry.MoveToLast();
            else
                return false;
        }

        public bool MoveToEntry(T entry)
        {
            foreach (var p in playlists)
                if (p.Contains(entry))
                {
                    if (playlists.MoveToEntry(p))
                        return p.MoveToEntry(entry);
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
            set { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
