using DeadDog.Audio.Libraries;

namespace DeadDog.Audio.Playlist
{
    public class LibraryPlaylist : TrackCollectionPlaylist
    {
        public LibraryPlaylist(Library library) : base(library.Tracks)
        {
            Library = library;
        }

        public Library Library { get; }
    }
}
