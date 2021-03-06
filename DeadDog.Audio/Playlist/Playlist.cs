﻿using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Playlist
{
    public class Playlist<T> : IPeekablePlaylist<T>, IPlaylist<T>, IList<T>
        where T : class
    {
        private T entry;
        private List<T> entries;
        private int index;
        private int tempIndex;

        public const int PreListIndex = -1;
        public const int PostListIndex = -2;
        public const int EmptyListIndex = -3;

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

                this.entry = value;

                if (EntryChanged != null)
                    EntryChanged(this, System.EventArgs.Empty);
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

        public Playlist()
        {
            this.entries = new List<T>();
            this.entry = null;
            this.index = EmptyListIndex;
            this.tempIndex = EmptyListIndex;
        }

        public event System.EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public void MoveEntry(T entry, int newIndex)
        {
            int oldIndex = IndexOf(entry);
            if (oldIndex == -1)
                throw new ArgumentOutOfRangeException(nameof(entry), "Move entry can only be executed when the entry is part of the playlist.");

            if (newIndex < 0 || newIndex >= entries.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            if (newIndex == oldIndex)
                return;

            var temp = entries[oldIndex];
            entries.RemoveAt(oldIndex);
            entries.Insert(newIndex, entry);

            if (index == oldIndex)
                index = newIndex;
            else if (oldIndex < index && newIndex >= index)
                index--;
            else if (oldIndex > index && newIndex <= index)
                index++;

            tempIndex = index;
        }

        public bool TryPeekNext(out T entry)
        {
            if (index == EmptyListIndex || index == PostListIndex || index == entries.Count - 1)
            {
                entry = default(T);
                return false;
            }
            else
            {
                entry = entries[index + 1];
                return true;
            }
        }

        public bool MoveNext()
        {
            if (index == EmptyListIndex)
                return false;

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
            if (index == EmptyListIndex)
                return false;

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
            if (index == EmptyListIndex)
                return;
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
            else if (this.index == EmptyListIndex)
                Index = PreListIndex;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= entries.Count)
                throw new ArgumentOutOfRangeException("index");

            if (index == this.index)
            {
                tempIndex = index;
                entries.RemoveAt(index);
                if (entries.Count == 0)
                    Index = EmptyListIndex;
                else if (index == entries.Count)
                    index = tempIndex = PostListIndex;
                else if (!TrySettingEntry(entries[tempIndex]))
                    MoveNext();
            }
            else if (index < this.index)
            {
                entries.RemoveAt(index);
                tempIndex = index--;
                if (entries.Count == 0)
                    Index = EmptyListIndex;
            }
            else
            {
                entries.RemoveAt(index);
                if (entries.Count == 0)
                    Index = EmptyListIndex;
            }
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
            if (index == EmptyListIndex)
                Index = PreListIndex;
        }

        public void Clear()
        {
            entries.Clear();
            Reset();
            Index = EmptyListIndex;
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
                if (entries.Count == 0)
                    Index = EmptyListIndex;
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
