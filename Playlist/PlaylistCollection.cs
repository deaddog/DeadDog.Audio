using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public abstract class PlaylistCollection<T> : IPlaylist<T>, IEnumerablePlaylist<T>, ISeekablePlaylist<T>
        where T : class
    {
        private List<IPlaylist<T>> playlists;
        private IPlaylist<T> currentList;
        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;

                IPlaylist<T> newList;
                if (value < 0)
                    newList = null;
                else
                    newList = playlists[value];

                if (currentList != null)
                {
                    currentList.EntryChanging -= list_EntryChanging;
                    currentList.EntryChanged -= list_EntryChanged;
                }
                if (newList != null)
                {
                    newList.EntryChanging -= list_EntryChanging;
                    newList.EntryChanged -= list_EntryChanged;
                }
                currentList = newList;
            }
        }

        private void list_EntryChanging(IPlaylist<T> sender, EntryChangingEventArgs<T> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
        }
        private void list_EntryChanged(object sender, EventArgs e)
        {
            if (EntryChanged != null)
                EntryChanged(this, e);
        }

        public PlaylistCollection()
        {
            this.playlists = new List<IPlaylist<T>>();
        }

        public T Entry
        {
            get { return currentList == null ? null : currentList.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public void Reset()
        {
            index = -1;
        }

        public bool MoveNext()
        {
            if (index == -2)
                return false;
            else if (playlists.Count == 0)
            {
                index = -2;
                return false;
            }
            else if (index == -1)
            {
                index = 0;
                playlists[index].Reset();
            }

            if (!playlists[index].MoveNext())
            {
                index++;
                if (index >= playlists.Count)
                    index = -2;
                else
                    playlists[index].Reset();
                return MoveNext();
            }
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
