using DeadDog.Audio.Libraries;

namespace DeadDog.Audio.Playlist
{
    public class AlbumPlaylist : TrackCollectionPlaylist
    {
        public AlbumPlaylist(Album album) : base(album.Tracks)
        {
            Album = album;
        }

        public Album Album { get; }
    }
}
