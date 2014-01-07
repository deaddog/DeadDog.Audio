namespace DeadDog.Audio
{
    public abstract class Playlist<T> : IPlaylist<T>
    {
        private T entry;

        public T Entry
        {
            get { return entry; }
        }

        public Playlist()
        {
            Reset();
        }

        public event System.EventHandler EntryChanged;
        public event EntryChangingEventHandler<T> EntryChanging;

        protected bool trySettingEntry(T entry, bool canReject)
        {
            if (EntryChanging != null)
            {
                EntryChangingEventArgs<T> e = new EntryChangingEventArgs<T>(entry, canReject);
                EntryChanging(this, e);
                if (e.Rejected)
                    return false;
            }

            this.entry = entry;
            if (EntryChanged != null)
                EntryChanged(this, System.EventArgs.Empty);
            return true;
        }

        public abstract bool MoveNext();

        public abstract void Reset();
    }
}
