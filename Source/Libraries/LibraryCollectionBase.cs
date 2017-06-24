using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DeadDog.Audio.Libraries
{
    public abstract class LibraryCollectionBase<T> : INotifyCollectionChanged, IEnumerable<T> where T : class
    {
        private readonly List<T> list;
        private readonly Comparison<T> _comparer;

        internal LibraryCollectionBase(Comparison<T> comparer)
        {
            this.list = new List<T>();
            _comparer = comparer;
        }

        internal abstract T _unknownElement { get; }

        private int FindIndexOf(T element) => list.BinarySearch(element, _comparer);

        public int Count
        {
            get { return list.Count; }
        }
        public T this[int index]
        {
            get { return list[index]; }
        }

        public bool Contains(T element)
        {
            return FindIndexOf(element) >= 0;
        }

        internal void Add(T element)
        {
            int index = FindIndexOf(element);
            if (index < 0) index = ~index;
            list.Insert(index, element);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index));
        }
        internal void Remove(T element)
        {
            int index = FindIndexOf(element);
            if (index >= 0)
            {
                list.RemoveAt(index);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element, index));
            }
        }

        internal void Reposition(T element)
        {
            list.Remove(element);

            int index = FindIndexOf(element);
            if (index < 0) index = ~index;
            list.Insert(index, element);
        }

        public int IndexOf(T element)
        {
            int index = FindIndexOf(element);
            return index < 0 ? -1 : index;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_unknownElement != null)
                yield return _unknownElement;
            T[] array = list.ToArray();
            for (int i = 0; i < array.Length; i++)
                yield return array[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }

        public override string ToString()
        {
            return "Count {" + list.Count + "}";
        }
    }
}
