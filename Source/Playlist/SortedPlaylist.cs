using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DeadDog.Audio.Playlist
{
    public class SortedPlaylist<T> : DecoratorPlaylist<T>, ICollection<T> where T : class
    {
        private readonly Playlist<T> _playlist;
        private readonly Comparison<T> _comparison;

        public SortedPlaylist(Comparison<T> comparison) : this(comparison, new Playlist<T>())
        {
        }
        private SortedPlaylist(Comparison<T> comparison, Playlist<T> playlist) : base(playlist)
        {
            _comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
            _playlist = playlist;
        }

        public new T Entry
        {
            get => _playlist.Entry;
            set => _playlist.Entry = value;
        }
        public bool TrySettingEntry(T entry) => _playlist.TrySettingEntry(entry);

        public int Index
        {
            get => _playlist.Index;
            set => _playlist.Index = value;
        }
        public bool TrySetIndex(int index) => _playlist.TrySetIndex(index);

        public int IndexOf(T item) => _playlist.IndexOf(item);
        public T this[int index] => _playlist[index];

        public int Count => _playlist.Count;

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var index = _playlist.BinarySearch(item, _comparison);
            if (index < 0) index = ~index;
            _playlist.Insert(index, item);

            if (item is INotifyPropertyChanged notif) notif.PropertyChanged += ElementPropertyChanged;
        }
        public bool Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            int index = _playlist.BinarySearch(item, _comparison);
            if (index < 0)
                return false;
            else
            {
                _playlist.RemoveAt(index);
                if (item is INotifyPropertyChanged notif) notif.PropertyChanged -= ElementPropertyChanged;
                return true;
            }
        }

        public bool Contains(T item) => _playlist.Contains(item);
        public void Clear()
        {
            foreach (var item in _playlist)
                if (item is INotifyPropertyChanged notif) notif.PropertyChanged -= ElementPropertyChanged;

            _playlist.Clear();
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e) => ElementPropertyChanged(sender as T);
        private void ElementPropertyChanged(T element)
        {
            int offset = 0;
            for (int i = 0; i < _playlist.Count; i++)
            {
                if (ReferenceEquals(element, _playlist[i]))
                    offset = -1;
                else if (_comparison(element, _playlist[i]) <= 0)
                {
                    _playlist.MoveEntry(element, i + offset);
                    return;
                }
            }

            _playlist.MoveEntry(element, _playlist.Count - 1);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => (_playlist as ICollection<T>).CopyTo(array, arrayIndex);
    }
}
