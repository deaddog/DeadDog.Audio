using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DeadDog.Audio.Libraries
{
    public abstract class LibraryCollectionBase<T> : INotifyCollectionChanged, IEnumerable<T> where T : class, INotifyPropertyChanged
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
            element.PropertyChanged += ElementPropertyChanged;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index));
        }
        internal void Remove(T element)
        {
            int index = FindIndexOf(element);
            if (index >= 0)
            {
                list.RemoveAt(index);
                element.PropertyChanged -= ElementPropertyChanged;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element, index));
            }
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is T element)
            {
                int oldIndex = -1;
                int newIndex = -1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (oldIndex == -1 && list[i] == element)
                        oldIndex = i;
                    else if (newIndex == -1 && _comparer(element, list[i]) < 0)
                        newIndex = i;
                }

                if (oldIndex == -1)
                    throw new ArgumentOutOfRangeException("An element on which a property has changed was not part of the collection.");
                if (newIndex == -1)
                    newIndex = list.Count;

                if (newIndex < oldIndex) //Move left
                {
                    list.RemoveAt(oldIndex);
                    list.Insert(newIndex, element);

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, element, newIndex - 1, oldIndex));
                }
                else if (newIndex > oldIndex) //Move right
                {
                    list.Insert(newIndex, element);
                    list.RemoveAt(oldIndex);

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, element, newIndex, oldIndex));
                }
            }
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
