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
        private List<IPlaylist<T>> playlists;
        private int index;
        private bool allowNullChange;

        public const int PreListIndex = -1;
        public const int PostListIndex = -2;
        public const int EmptyListIndex = -3;

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

        private void list_EntryChanging(IPlayable<T> sender, EntryChangingEventArgs<T> e)
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
                return false;
            }

            return true;
        }
        public bool MovePrevious()
        {
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
                moved = playlists[tempIndex].MovePrevious();
                if (!moved)
                {
                    tempIndex++;
                    if (tempIndex >= playlists.Count)
                        tempIndex = PostListIndex;
                    else
                        moved = !playlists[tempIndex].MoveToLast();
                }
            }

            allowNullChange = true;

            if (tempIndex == PostListIndex && EntryChanged != null)
            {
                index = tempIndex;
                EntryChanged(this, EventArgs.Empty);
                return false;
            }

            return true;
        }

        public bool MoveToFirst()
        {
            if (playlists.Count == 0)
                return false;
            else
                return playlists[0].MoveToFirst();
        }
        public bool MoveToLast()
        {
            if (playlists.Count == 0)
                return false;
            else
                return playlists[playlists.Count - 1].MoveToLast();
        }

        public bool MoveToEntry(T entry)
        {
            if (index == -1 || index == -2)
                index = 0;

            for (int i = 0; i < playlists.Count; i++)
                if (playlists[(i + index) % playlists.Count].Contains(entry))
                    return playlists[(i + index) % playlists.Count].MoveToEntry(entry);

            return false;
        }
        public bool Contains(T entry)
        {
            for (int i = 0; i < playlists.Count; i++)
                if (playlists[i].Contains(entry))
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

        public void Add(IPlaylist<T> playlist)
        {
            playlists.Add(playlist);
            playlist.EntryChanging += list_EntryChanging;
            playlist.EntryChanged += list_EntryChanged;
        }
        public void Insert(int index, IPlaylist<T> playlist)
        {
            playlists.Insert(index, playlist);
            if (index <= this.index)
                this.index++;

            playlist.EntryChanging += list_EntryChanging;
            playlist.EntryChanged += list_EntryChanged;
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
            if (index < 0 || index >= playlists.Count)
                throw new ArgumentOutOfRangeException("index");

            IPlaylist<T> playlist = playlists[index];
            if (index == this.index)
            {
                playlists.RemoveAt(index);
                if (this.index == playlists.Count)
                    this.index = PostListIndex;
                else
                {
                    allowNullChange = false;
                    playlists[this.index].Reset();
                    allowNullChange = true;
                    MoveNext();
                }
            }
            else if (index < this.index)
            {
                playlists.RemoveAt(index);
                index--;
            }
            else
                playlists.RemoveAt(index);

            playlist.EntryChanging -= list_EntryChanging;
            playlist.EntryChanged -= list_EntryChanged;
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
