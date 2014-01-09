namespace DeadDog.Audio
{
    public abstract class Playlist<T> : IPlaylist<T>
    {
        private T entry;

        public T Entry
        {
            get { return entry; }
            protected set
            {
                this.entry = value;

                if (EntryChanged != null)
                    EntryChanged(this, System.EventArgs.Empty);
            }
        }

        public Playlist()
        {
            Reset();
        }

        public event System.EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        protected bool trySettingEntry(T entry)
        {
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

        public abstract void Reset();

        public abstract bool MoveNext();
    }
}
