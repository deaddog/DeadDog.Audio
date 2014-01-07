namespace DeadDog.Audio
{
    public abstract class Playlist<T> : IPlaylist<T>
    {
        public T Entry
        {
            get { throw new System.NotImplementedException(); }
        }

        public event EntryChangedEventHandler<T> EntryChanged;

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
