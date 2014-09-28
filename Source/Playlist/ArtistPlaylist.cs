using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class ArtistPlaylist : IPlaylist<Track>, IEnumerablePlaylist<Track>, ISeekablePlaylist<Track>
    {
        private Artist artist;
        private Dictionary<Album, AlbumPlaylist> lookup;
        private PlaylistCollection<Track> playlists;

        public ArtistPlaylist(Artist artist)
        {
            this.artist = artist;
            this.lookup = new Dictionary<Album, AlbumPlaylist>();
            this.playlists = new PlaylistCollection<Track>();

            this.playlists.EntryChanged += playlists_EntryChanged;
            this.playlists.EntryChanging += playlists_EntryChanging;

            foreach (var album in artist.Albums)
            {
                AlbumPlaylist albumplaylist = new AlbumPlaylist(album);
                lookup.Add(album, albumplaylist);
                playlists.Add(albumplaylist);
            }

            this.artist.Albums.AlbumAdded += new AlbumEventHandler(Albums_AlbumAdded);
            this.artist.Albums.AlbumRemoved += new AlbumEventHandler(Albums_AlbumRemoved);
        }

        void playlists_EntryChanged(object sender, EventArgs e)
        {
            if (EntryChanged != null)
                EntryChanged(this, e);
        }
        void playlists_EntryChanging(IPlaylist<Track> sender, EntryChangingEventArgs<Track> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
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

        public Track Entry
        {
            get { return playlists.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<Track> EntryChanging;

        public void Reset()
        {
            playlists.Reset();
        }

        public bool MoveNext()
        {
            return playlists.MoveNext();
        }
        public bool MovePrevious()
        {
            return playlists.MovePrevious();
        }

        public bool MoveToFirst()
        {
            return playlists.MoveToFirst();
        }
        public bool MoveToLast()
        {
            return playlists.MoveToLast();
        }

        public bool MoveToEntry(Track entry)
        {
            return playlists.MoveToEntry(entry);
        }
        public bool Contains(Track entry)
        {
            return playlists.Contains(entry);
        }

        public IEnumerator<Track> GetEnumerator()
        {
            return (playlists as IEnumerable<Track>).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (playlists as IEnumerable<Track>).GetEnumerator();
        }
    }
}
