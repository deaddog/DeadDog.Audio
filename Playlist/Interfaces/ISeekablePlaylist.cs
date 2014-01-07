namespace DeadDog.Audio
{
    public interface ISeekablePlaylist<T> : IPlaylist<T>
    {
        bool MoveToEntry(T entry);
        bool Contains(T entry);
    }
}
