namespace DeadDog.Audio
{
    public abstract class Playlist<T> : IPlaylist<T>
    {
        public T Entry
        {
            get { throw new System.NotImplementedException(); }
        }

        public event System.EventHandler EntryChanged;
        public event EntryChangedEventHandler<T> EntryChanging;

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
