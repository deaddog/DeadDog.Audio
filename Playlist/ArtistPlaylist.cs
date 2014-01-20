using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class ArtistPlaylist : PlaylistCollection<Track>
    {
        private Artist artist;
        private Dictionary<Album, AlbumPlaylist> lookup;

        public ArtistPlaylist(Artist artist)
        {
            this.artist = artist;
            lookup = new Dictionary<Album, AlbumPlaylist>();

            foreach (var album in artist.Albums)
            {
                AlbumPlaylist albumplaylist = new AlbumPlaylist(album);
                lookup.Add(album, albumplaylist);
                addPlaylist(albumplaylist);
            }

            this.artist.Albums.AlbumAdded += new AlbumEventHandler(Albums_AlbumAdded);
            this.artist.Albums.AlbumRemoved += new AlbumEventHandler(Albums_AlbumRemoved);

        }

        void Albums_AlbumRemoved(Album.AlbumCollection collection, AlbumEventArgs e)
        {
            if (lookup.ContainsKey(e.Album))
            {
                removePlaylist(lookup[e.Album]);
                lookup.Remove(e.Album);
            }
            else throw new InvalidOperationException("Playlist was not in the ArtistPlaylist and could not be removed");
        }

        void Albums_AlbumAdded(Album.AlbumCollection collection, AlbumEventArgs e)
        {
            AlbumPlaylist albumplaylist = new AlbumPlaylist(e.Album);
            lookup.Add(e.Album, albumplaylist);
            addPlaylist(albumplaylist);
        }
    }
}
