using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Playlist
{
    public abstract class DecoratorPlaylist<T> : IPeekablePlaylist<T>, IPlaylist<T>
    {
        private readonly IPlaylist<T> _playlist;

        public DecoratorPlaylist(IPlaylist<T> playlist)
        {
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));

            _playlist.EntryChanged += (s, e) => EntryChanged?.Invoke(this, e);
            _playlist.EntryChanging += (s, e) => EntryChanging?.Invoke(this, e);
        }

        public T Entry => _playlist.Entry;

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public virtual void Reset() => _playlist.Reset();

        public virtual bool TryPeekNext(out T entry)
        {
            if (_playlist is IPeekablePlaylist<T> peek && peek.TryPeekNext(out entry))
                return true;
            else
            {
                entry = default(T);
                return false;
            }
        }

        public virtual bool MoveNext() => _playlist.MoveNext();
        public virtual bool MovePrevious() => _playlist.MovePrevious();

        public virtual bool MoveToFirst() => _playlist.MoveToFirst();
        public virtual bool MoveToLast() => _playlist.MoveToLast();

        public virtual bool MoveToEntry(T entry) => _playlist.MoveToEntry(entry);
        public virtual bool Contains(T entry) => _playlist.Contains(entry);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (_playlist as IEnumerable<T>).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_playlist as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
