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
                index = value;

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
                this.index = listIndex;

                if (EntryChanged != null)
                    EntryChanged(this, e);
            }
        }

        public PlaylistCollection()
        {
            this.playlists = new List<IPlaylist<T>>();
            this.index = PreListIndex;
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
            this.index = PreListIndex;
        }

        public bool MoveNext()
        {
            int tempIndex = this.index;

            if (tempIndex == PostListIndex)
                return false;
            if (playlists.Count == 0)
            {
                tempIndex = PostListIndex;
                return false;
            }

            allowNullChange = false;

            if (tempIndex == PreListIndex)
            {
                tempIndex = 0;
                playlists[tempIndex].Reset();
            }

            while (tempIndex != PostListIndex && !playlists[tempIndex].MoveNext())
            {
                tempIndex++;
                if (tempIndex >= playlists.Count)
                    tempIndex = PostListIndex;
                else
                    playlists[tempIndex].Reset();
            }

            allowNullChange = true;

            if (tempIndex == PostListIndex && EntryChanged != null)
            {
                index = tempIndex;
                EntryChanged(this, EventArgs.Empty);
            }

            return true;
        }
        public bool MovePrevious()
        {
            if (!IsIEnumerablePlaylist)
                throw new InvalidOperationException("Cannot perform MovePrevious, as one or more inner playlists does not implement IEnumerablePlaylist.");

            int tempIndex = this.index;

            if (tempIndex == PreListIndex)
                return false;
            if (playlists.Count == 0)
            {
                tempIndex = PreListIndex;
                return false;
            }

            allowNullChange = false;

            if (tempIndex == PostListIndex)
            {
                tempIndex = playlists.Count - 1;
                playlists[tempIndex].Reset();
            }

            bool moved = false;

            while (tempIndex != PreListIndex && !moved)
            {
                moved = (playlists[tempIndex] as IEnumerablePlaylist<T>).MovePrevious();
                if (!moved)
                {
                    tempIndex++;
                    if (tempIndex >= playlists.Count)
                        tempIndex = PostListIndex;
                    else
                        moved = !(playlists[tempIndex] as IEnumerablePlaylist<T>).MoveToLast();
                }
            }

            allowNullChange = true;

            if (tempIndex == PostListIndex && EntryChanged != null)
            {
                index = tempIndex;
                EntryChanged(this, EventArgs.Empty);
            }

            return true;
        }

        public bool MoveToFirst()
        {
            if (!IsIEnumerablePlaylist)
                throw new InvalidOperationException("Cannot perform MoveToFirst, as one or more inner playlists does not implement IEnumerablePlaylist.");

            if (playlists.Count == 0)
                return false;
            else
                return (playlists[0] as IEnumerablePlaylist<T>).MoveToFirst();
        }
        public bool MoveToLast()
        {
            if (!IsIEnumerablePlaylist)
                throw new InvalidOperationException("Cannot perform MoveToLast, as one or more inner playlists does not implement IEnumerablePlaylist.");

            if (playlists.Count == 0)
                return false;
            else
                return (playlists[playlists.Count - 1] as IEnumerablePlaylist<T>).MoveToLast();
        }

        public bool MoveToEntry(T entry)
        {
            if (!IsISeekablePlaylist)
                throw new InvalidOperationException("Cannot perform MoveToEntry, as one or more inner playlists does not implement ISeekablePlaylist.");

            for (int i = 0; i < playlists.Count; i++)
                if ((playlists[(i + index) % playlists.Count] as ISeekablePlaylist<T>).MoveToEntry(entry))
                    return true;

            return false;
        }
        public bool Contains(T entry)
        {
            if (!IsISeekablePlaylist)
                throw new InvalidOperationException("Cannot perform Contains, as one or more inner playlists does not implement ISeekablePlaylist.");

            for (int i = 0; i < playlists.Count; i++)
                if ((playlists[(i + index) % playlists.Count] as ISeekablePlaylist<T>).Contains(entry))
                    return true;

            return false;
        }

        public bool IsIEnumerablePlaylist
        {
            get
            {
                foreach (var p in playlists)
                    if (!(p is IEnumerablePlaylist<T>))
                        return false;
                return true;
            }
        }
        public bool IsISeekablePlaylist
        {
            get
            {
                foreach (var p in playlists)
                    if (!(p is ISeekablePlaylist<T>))
                        return false;
                return true;
            }
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
            int i = playlists.IndexOf(playlist);
            if (i < 0)
                return false;

            if (i == this.index)
            {
                playlists.RemoveAt(i);
                if (index == playlists.Count)
                    index = PostListIndex;
                else
                {
                    allowNullChange = false;
                    playlists[index].Reset();
                    allowNullChange = true;
                    MoveNext();
                }
                return true;
            }
            else if (i < this.index)
            {
                playlists.RemoveAt(i);
                index--;
            }
            else
                playlists.RemoveAt(i);
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
