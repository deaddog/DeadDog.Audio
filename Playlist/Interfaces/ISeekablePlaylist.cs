namespace DeadDog.Audio.Playlist.Interfaces
{
    public interface ISeekablePlaylist<T> : IPlaylist<T>
    {
        bool MoveToEntry(T entry);
        bool Contains(T entry);
    }
}
