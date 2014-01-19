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
    public abstract class PlaylistCollection<T> : IPlaylist<T>, IEnumerablePlaylist<T>, ISeekablePlaylist<T>
        where T : class
    {
        private List<IPlaylist<T>> playlists;
        private int index;
        private int tempIndex;
        private bool allowNullChange;

        public const int PreListIndex = -1;
        public const int PostListIndex = -2;

        public int Index
        {
            get { return index; }
            set
            {
                if (value < 0 || value >= playlists.Count)
                    throw new ArgumentOutOfRangeException("value");

                T entry = Entry;
                index = tempIndex = value;

                if (entry != Entry && EntryChanged != null)
                    EntryChanged(this, EventArgs.Empty);
            }
        }

        private void list_EntryChanging(IPlaylist<T> sender, EntryChangingEventArgs<T> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
        }
        private void list_EntryChanged(object sender, EventArgs e)
        {
            if (!(sender is IPlaylist<T>))
                throw new ArgumentException("Event sender must be a playlist.", "sender");

            IPlaylist<T> playlist = sender as IPlaylist<T>;
            if (!playlists.Contains(playlist))
                throw new ArgumentException("Event sender must be a playlist contained by the PlaylistCollection.", "sender");

            if (allowNullChange || playlist.Entry != null)
            {
                int listIndex = playlists.IndexOf(playlist);
                this.index = tempIndex = listIndex;

                if (EntryChanged != null)
                    EntryChanged(this, e);
            }
        }

        public PlaylistCollection()
        {
            this.playlists = new List<IPlaylist<T>>();
            this.index = PreListIndex;
            this.tempIndex = PreListIndex;
            this.allowNullChange = true;
        }

        public T Entry
        {
            get { return index < 0 ? null : playlists[index].Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public void Reset()
        {
            this.index = tempIndex = PreListIndex;
        }

        public bool MoveNext()
        {
            if (index == PostListIndex)
                return false;
            if (playlists.Count == 0)
            {
                index = PostListIndex;
                return false;
            }

            allowNullChange = false;

            if (index == PreListIndex)
            {
                index = 0;
                playlists[index].Reset();
            }

            while (index != PostListIndex && !playlists[index].MoveNext())
            {
                index++;
                if (index >= playlists.Count)
                    index = PostListIndex;
                else
                    playlists[index].Reset();
            }

            allowNullChange = true;

            if (index == PostListIndex && EntryChanged != null)
                EntryChanged(this, EventArgs.Empty);

            return true;
        }
        public bool MovePrevious()
        {
            if (index == -1)
                return false;

            else if (playlists.Count == 0)
            {
                index = -1;
                return false;
            }
            else if (index == -2)
            {
                index = playlists.Count - 1;
                playlists[index].Reset();
            }

            if (!(playlists[index] is IEnumerablePlaylist<T>))
                throw new InvalidOperationException("Cannot perform MovePrevious, as inner playlist does not implement IEnumerablePlaylist.");

            throw new NotImplementedException();
        }

        public bool MoveToFirst()
        {
            throw new NotImplementedException();
        }
        public bool MoveToLast()
        {
            throw new NotImplementedException();
        }
        public bool MoveToEntry(T entry)
        {
            throw new NotImplementedException();
        }

        public bool Contains(T entry)
        {
            throw new NotImplementedException();
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
            int i = playlists.IndexOf(playlist);

            if (i == -1)
                return false;

            IPlaylist<T> selected = index < 0 ? null : playlists[this.index];

            playlists.RemoveAt(i);
            playlists.Insert(index, playlist);

            if (selected != null)
                this.index = playlists.IndexOf(selected);
            return true;
        }

        public void Insert(int index, IPlaylist<T> playlist)
        {
            playlists.Insert(index, playlist);
            if (index <= this.index)
                this.index++;
        }
        public bool Remove(IPlaylist<T> playlist)
        {
            bool removed = this.playlists.Remove(playlist);
            if (index >= playlists.Count)
                index = -2;
            else
                playlists[index].Reset();
            return removed;
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (IPlaylist<T> playlist in playlists)
                if (!(playlist is IEnumerablePlaylist<T>))
                    throw new InvalidOperationException("To enumerate a PlaylistCollection, all inner playlists must implement the IEnumerablePlaylist interface.");

            foreach (IPlaylist<T> playlist in playlists)
                foreach (T t in (playlist as IEnumerablePlaylist<T>))
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
