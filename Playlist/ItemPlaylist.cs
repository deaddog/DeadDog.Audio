using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public class ItemPlaylist<T> : IPlaylist<T>, IEnumerablePlaylist<T>, ISeekablePlaylist<T>, IList<T>
        where T : class
    {
        private T entry;
        private List<T> entries;
        private int index;
        private int tempIndex;

        public const int PreListIndex = -1;
        public const int PostListIndex = -2;

        public T Entry
        {
            get { return entry; }
            set
            {
                if (value != null)
                    if (!entries.Contains(value))
                        throw new KeyNotFoundException("The supplied entry was not found in the playlist.");

                T indexEntry = tempIndex < 0 ? null : entries[tempIndex];
                if (indexEntry == value)
                    this.index = this.tempIndex;
                else if (value == null)
                    this.index = this.tempIndex = PreListIndex;
                else
                    this.index = this.tempIndex = entries.IndexOf(value);

                if (this.entry != value)
                {
                    this.entry = value;

                    if (EntryChanged != null)
                        EntryChanged(this, System.EventArgs.Empty);
                }
            }
        }
        public bool TrySettingEntry(T entry)
        {
            if (entry != null)
                if (!entries.Contains(entry))
                    throw new KeyNotFoundException("The supplied entry was not found in the playlist.");

            if (EntryChanging != null)
            {
                EntryChangingEventArgs<T> e = new EntryChangingEventArgs<T>(entry);
                EntryChanging(this, e);
                if (e.Rejected)
                    return false;
            }

            this.Entry = entry;
            return true;
        }

        public int Index
        {
            get { return index; }
            set
            {
                tempIndex = value;

                if (value == PreListIndex)
                    Entry = null;
                else if (value == PostListIndex)
                    Entry = null;
                else if (value < 0 || value >= entries.Count)
                    throw new ArgumentOutOfRangeException("value", "Index must be greater than or equal to zero and less than the Count property. Use PreListIndex and PostListIndex for null value.");
                else
                    Entry = entries[value];
            }
        }
        public bool TrySetIndex(int index)
        {
            int temp = this.tempIndex;

            this.tempIndex = index;
            bool couldSet;

            if (index == PreListIndex)
                couldSet = TrySettingEntry(null);
            else if (index == PostListIndex)
                couldSet = TrySettingEntry(null);
            else if (index < 0 || index >= entries.Count)
                throw new ArgumentOutOfRangeException("value", "Index must be greater than or equal to zero and less than the Count property. Use PreListIndex and PostListIndex for null value.");
            else
                couldSet = TrySettingEntry(entries[index]);

            if (!couldSet)
                this.tempIndex = temp;
            return couldSet;
        }

        public ItemPlaylist()
        {
            this.entries = new List<T>();
            this.entry = null;
            this.index = PreListIndex;
            this.tempIndex = PreListIndex;
        }

        public event System.EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public bool MoveNext()
        {
            if (index == PostListIndex)
                return false;

            tempIndex++;
            if (tempIndex < entries.Count)
            {
                if (TrySettingEntry(entries[tempIndex]))
                    return true;
                else
                    return MoveNext();
            }
            else
            {
                Index = PostListIndex;
                return false;
            }
        }
        public bool MovePrevious()
        {
            if (index == PreListIndex)
                return false;

            tempIndex = tempIndex == PostListIndex ? entries.Count - 1 : tempIndex - 1;
            if (tempIndex >= 0)
            {
                if (TrySettingEntry(entries[tempIndex]))
                    return true;
                else
                    return MovePrevious();
            }
            else
            {
                Index = PreListIndex;
                return false;
            }
        }

        public bool MoveToFirst()
        {
            if (entries.Count == 0)
                return false;
            else if (index == 0)
                return true;
            else
            {
                Index = 0;
                return true;
            }
        }
        public bool MoveToLast()
        {
            if (entries.Count == 0)
                return false;
            else if (index == entries.Count - 1)
                return true;
            else
            {
                Index = entries.Count - 1;
                return true;
            }
        }
        public bool MoveToEntry(T entry)
        {
            if (entry == null)
            {
                Index = PostListIndex;
                return true;
            }

            int i = entries.IndexOf(entry);
            if (i == -1)
                return false;
            else
            {
                Index = i;
                return true;
            }
        }

        public void Reset()
        {
            this.Index = PreListIndex;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return entries.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            entries.Insert(index, item);
            if (index <= this.index)
                this.tempIndex = this.index++;
        }

        public void RemoveAt(int index)
        {
            T entry = entries[index];
            if (index == this.index)
            {
                tempIndex = index;
                entries.RemoveAt(index);
                if (!TrySettingEntry(entries[tempIndex]))
                    MoveNext();
            }
            else if (index < this.index)
            {
                entries.RemoveAt(index);
                tempIndex = index--;
            }
            else
                entries.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return entries[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                entries[index] = value;
                if (!TrySettingEntry(value))
                    MoveNext();
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            entries.Add(item);
        }

        public void Clear()
        {
            entries.Clear();
            Reset();
        }

        public bool Contains(T item)
        {
            return entries.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            entries.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return entries.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

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

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T e in entries)
                yield return e;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (T e in entries)
                yield return e;
        }

        #endregion
    }
}
