using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public class Playlist<T> : IList<PlaylistEntry<T>>
    {
        /// <summary>
        /// The full list of all entries in the actual playlist.
        /// </summary>
        private List<PlaylistEntry<T>> entries;
        /// <summary>
        /// Contains the tracks that have been played already - used for the <see cref="MovePrevious"/> method.
        /// </summary>
        private ReverseList<T> reverselist;
        /// <summary>
        /// A list of the tracks that have not been played yet (in shuffle mode) - used to ensure that everything is played once before repeating.
        /// </summary>
        private List<PlaylistEntry<T>> shufflelist;

        private IPlayQueue<T> queue;
        private PlaylistEntry<T> currentEntry = null;
        private RepeatTypes repeat = RepeatTypes.Off;
        private RestrictionTypes restriction = RestrictionTypes.None;
        private ShuffleTypes shuffle = ShuffleTypes.Off;

        public T Current
        {
            get
            {
                if (currentEntry == null)
                    return default(T);
                else
                    return currentEntry.Track;
            }
        }

        public PlaylistEntry<T> CurrentEntry
        {
            get { return currentEntry; }
        }
        public int CurrentIndex
        {
            get { return entries.IndexOf(currentEntry); }
        }

        public IPlayQueue<T> Queue
        {
            get { return queue; }
        }

        public RepeatTypes Repeat
        {
            get { return repeat; }
            set { this.repeat = value; }
        }
        public RestrictionTypes Restriction
        {
            get { return restriction; }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ShuffleTypes Shuffle
        {
            get { return this.shuffle; }
            set
            {
                if (this.shuffle == value)
                    return;
                this.shuffle = value;
                if (this.shuffle == ShuffleTypes.Shuffle)
                {
                    shufflelist.Clear();
                    shufflelist.AddRange(entries);
                    shufflelist.Remove(currentEntry);
                    reverselist.Clear();
                }
            }
        }

        public Playlist(IPlayQueue<T> queue)
        {
            this.queue = queue;
            entries = new List<PlaylistEntry<T>>();
            reverselist = new ReverseList<T>();
            shufflelist = new List<PlaylistEntry<T>>();
        }

        private bool nextS()
        {
            if (shufflelist.Count == 0)
            {
                if (repeat == RepeatTypes.Repeat)
                    shufflelist.AddRange(entries);
                else
                    return false;
            }

            Random rnd = new Random(DateTime.Now.Millisecond);
            int i = rnd.Next(shufflelist.Count);
            currentEntry = shufflelist[i];
            shufflelist.RemoveAt(i);
            return true;
        }
        private bool nextR()
        {
            int i = entries.IndexOf(currentEntry);
            i = (i + 1) % entries.Count;
            currentEntry = entries[i];
            return true;
        }
        private bool next()
        {
            int i = entries.IndexOf(currentEntry) + 1;
            if (i >= entries.Count)
                return false;

            currentEntry = entries[i];
            return true;
        }

        public bool MoveNext()
        {
            if (queue.Count > 0)
            {
                PlaylistEntry<T> e = queue.Dequeue();
                currentEntry = e;
                if (shuffle == ShuffleTypes.Shuffle)
                {
                    reverselist.Add(e);
                    shufflelist.Remove(e);
                }
                return true;
            }

            if (shuffle == ShuffleTypes.Shuffle)
            {
                if (reverselist.MoveNext())
                {
                    currentEntry = reverselist.Current;
                    return true;
                }
                else
                    reverselist.Add(currentEntry);
            }

            bool b;
            if (shuffle == ShuffleTypes.Shuffle)
                b = nextS();
            else if (repeat == RepeatTypes.Repeat)
                b = nextR();
            else
                b = next();

            return b;
        }
        public bool MovePrevious()
        {
            if (shuffle == ShuffleTypes.Shuffle)
            {
                PlaylistEntry<T> e = currentEntry;
                if (reverselist.Current == null)
                {
                    reverselist.Add(e);
                    reverselist.MovePrevious();
                    reverselist.CanRemoveTop = true;
                }
                if (reverselist.MovePrevious())
                {
                    currentEntry = reverselist.Current;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                int i = entries.IndexOf(currentEntry) - 1;
                if (i < 0 && repeat == RepeatTypes.Off)
                    return false;
                else if (i < 0)
                {
                    i = entries.Count - 1;
                }
                currentEntry = entries[i];
                return true;
            }
        }
        public bool MoveTo(PlaylistEntry<T> entry)
        {
            return MoveTo(entries.IndexOf(entry));
        }
        public bool MoveTo(int index)
        {
            if (index < 0 || index >= entries.Count)
                return false;

            PlaylistEntry<T> e = entries[index];
            currentEntry = e;
            if (shuffle == ShuffleTypes.Shuffle)
                reverselist.Add(e);

            return true;
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
            reverselist.Remove(entry);
            shufflelist.Remove(entry);
            queue.Remove(entry);
            if (index == entries.IndexOf(currentEntry))
                currentEntry = null;
            entries.Remove(entry);
        }

        public PlaylistEntry<T> this[int index]
        {
            get { return entries[index]; }
            set
            {
                throw new InvalidOperationException("Property cannot be set.");
            }
        }

        #endregion

        #region ICollection<PlaylistEntry<T>> Members

        public void Add(PlaylistEntry<T> item)
        {
            entries.Add(item);
            if (shuffle == ShuffleTypes.Shuffle)
                shufflelist.Add(item);
        }

        public void Clear()
        {
            reverselist.Clear();
            shufflelist.Clear();
            queue.Clear();
            entries.Clear();
            currentEntry = null;
        }

        public bool Contains(PlaylistEntry<T> item)
        {
            return entries.Contains(item);
        }

        public void CopyTo(PlaylistEntry<T>[] array, int arrayIndex)
        {
            entries.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return entries.Count; }
        }

        public bool IsReadOnly
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

        public IEnumerator<PlaylistEntry<T>> GetEnumerator()
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
