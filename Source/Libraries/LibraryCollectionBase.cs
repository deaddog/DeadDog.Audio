using System.Collections.Generic;

namespace DeadDog.Audio.Libraries
{
    public abstract class LibraryCollectionBase<T> : IEnumerable<T> where T : class
    {
        private List<T> list;

        internal LibraryCollectionBase()
        {
            this.list = new List<T>();
        }

        protected abstract int Compare(T element1, T element2);

        internal abstract T _unknownElement { get; }

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
            return list.BinarySearch(element, Compare) >= 0;
        }

        internal void Add(T element)
        {
            int index = list.BinarySearch(element, Compare);
            if (index < 0) index = ~index;
            list.Insert(index, element);

            OnAdded(element);
        }
        internal void Remove(T element)
        {
            list.Remove(element);

            OnRemoved(element);
        }

        internal void Reposition(T element)
        {
            list.Remove(element);

            int index = list.BinarySearch(element, Compare);
            if (index < 0) index = ~index;
            list.Insert(index, element);
        }

        public int IndexOf(T element)
        {
            int index = list.BinarySearch(element, Compare);
            return index < 0 ? -1 : index;
        }

        protected abstract void OnAdded(T element);
        protected abstract void OnRemoved(T element);

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
