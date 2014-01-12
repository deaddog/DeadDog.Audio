using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public class ItemPlaylist<T> : Playlist<T>, IEnumerablePlaylist<T>, ISeekablePlaylist<T>, IList<T>
        where T : class
    {
        private List<PlaylistEntry<T>> entries;
        private int index = -1;

        public int CurrentIndex
        {
            get { return index; }
        }

        public ItemPlaylist()
        {
            this.entries = new List<PlaylistEntry<T>>();
        }

        public bool MoveNext()
        {
            if (index == -2)
                return false;

            index++;
            if (index >= entries.Count)
            {
                index = -2;
                return false;
            }
            else
                return true;
        }
        public bool MovePrevious()
        {
            if (index == -2)
                return false;

            index--;
            if (index == -1)
            {
                index = -2;
                return false;
            }
            else
                return true;
        }

        public bool MoveToFirst()
        {
            if (entries.Count == 0)
            {
                index = -2;
                return false;
            }
            else
            {
                index = 0;
                return true;
            }
        }
        public bool MoveToLast()
        {
            if (entries.Count == 0)
            {
                index = -2;
                return false;
            }
            else
            {
                index = entries.Count - 1;
                return true;
            }
        }
        public bool MoveToEntry(PlaylistEntry<T> entry)
        {
            int i = entries.IndexOf(entry);
            if (i == -1)
            {
                index = -2;
                return false;
            }
            else
            {
                index = i;
                return true;
            }
        }

        public void Reset()
        {
            this.index = -1;
        }

        #region IList<PlaylistEntry<T>> Members

        public int IndexOf(T item)
        {
            return entries.FindIndex(x => x.Track.Equals(item));
        }
        public int IndexOf(PlaylistEntry<T> item)
        {
            return entries.IndexOf(item);
        }

        public void Insert(int index, PlaylistEntry<T> item)
        {
            entries.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            PlaylistEntry<T> entry = entries[index];
            if (index == this.index && index >= entries.Count - 1)
                this.index = -2;
            else if (index > this.index)
                this.index--;
            entries.Remove(entry);
        }

        PlaylistEntry<T> IList<PlaylistEntry<T>>.this[int index]
        {
            get { return this[index]; }
            set { throw new InvalidOperationException("Property cannot be set."); }
        }

        public PlaylistEntry<T> this[int index]
        {
            get { return entries[index]; }
        }

        #endregion

        #region ICollection<PlaylistEntry<T>> Members

        public void Add(PlaylistEntry<T> item)
        {
            entries.Add(item);
        }

        public void Clear()
        {
            entries.Clear();
            index = -2;
        }

        public bool Contains(PlaylistEntry<T> item)
        {
            return entries.Contains(item);
        }

        void ICollection<PlaylistEntry<T>>.CopyTo(PlaylistEntry<T>[] array, int arrayIndex)
        {
            entries.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return entries.Count; }
        }

        bool ICollection<PlaylistEntry<T>>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PlaylistEntry<T> item)
        {
            int index = entries.IndexOf(item);
            if (index == -1)
                return false;
            else
            {
                this.RemoveAt(index);
                return true;
            }
        }

        #endregion

        #region IEnumerable<PlaylistEntry<T>> Members

        IEnumerator<PlaylistEntry<T>> IEnumerable<PlaylistEntry<T>>.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        #endregion
    }
}
