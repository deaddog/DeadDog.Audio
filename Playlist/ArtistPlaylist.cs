using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class ArtistPlaylist : IPlaylist<Track>
    {
        private Artist artist;
        private Dictionary<Album, AlbumPlaylist> lookup;
        private PlaylistCollection<Track> playlists;

        public ArtistPlaylist(Artist artist)
        {
            this.artist = artist;
            this.lookup = new Dictionary<Album, AlbumPlaylist>();
            this.playlists = new PlaylistCollection<Track>();

            foreach (var album in artist.Albums)
            {
                AlbumPlaylist albumplaylist = new AlbumPlaylist(album);
                lookup.Add(album, albumplaylist);
                playlists.Add(albumplaylist);
            }

            this.artist.Albums.AlbumAdded += new AlbumEventHandler(Albums_AlbumAdded);
            this.artist.Albums.AlbumRemoved += new AlbumEventHandler(Albums_AlbumRemoved);
        }

        void Albums_AlbumRemoved(Album.AlbumCollection collection, AlbumEventArgs e)
        {
            if (lookup.ContainsKey(e.Album))
            {
                playlists.Remove(lookup[e.Album]);
                lookup.Remove(e.Album);
            }
            else
                throw new InvalidOperationException("Playlist was not in the ArtistPlaylist and could not be removed");
        }

        void Albums_AlbumAdded(Album.AlbumCollection collection, AlbumEventArgs e)
        {
            AlbumPlaylist albumplaylist = new AlbumPlaylist(e.Album);
            lookup.Add(e.Album, albumplaylist);
            playlists.Add(albumplaylist);
        }
    }
}
