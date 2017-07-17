using System;

namespace DeadDog.Audio.Playlist
{
    public abstract class DecoratorPlayable<T> : IPlayable<T>
    {
        private readonly IPlayable<T> _playable;

        public DecoratorPlayable(IPlayable<T> playable)
        {
            _playable = playable ?? throw new ArgumentNullException(nameof(playable));

            _playable.EntryChanged += (s, e) => EntryChanged?.Invoke(this, e);
            _playable.EntryChanging += (s, e) => EntryChanging?.Invoke(this, e);
        }

        public T Entry => _playable.Entry;

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        public virtual bool MoveNext() => _playable.MoveNext();
    }
}
