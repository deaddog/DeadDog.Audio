using DeadDog.Audio.Libraries;
using System;
using System.Collections.Generic;

namespace DeadDog.Audio
{
    public class LibraryPlaylist : IPlaylist<Track>, IEnumerablePlaylist<Track>, ISeekablePlaylist<Track>
    {
        private Library library;
        private Dictionary<Artist, ArtistPlaylist> lookup;
        private PlaylistCollection<Track> playlists;

        public LibraryPlaylist(Library library)
        {
            this.library = library;
            this.lookup = new Dictionary<Artist, ArtistPlaylist>();
            this.playlists = new PlaylistCollection<Track>();

            this.playlists.EntryChanged += playlists_EntryChanged;
            this.playlists.EntryChanging += playlists_EntryChanging;

            foreach (var artist in library.Artists)
            {
                ArtistPlaylist artistplaylist = new ArtistPlaylist(artist);
                lookup.Add(artist, artistplaylist);
                playlists.Add(artistplaylist);
            }

            this.library.Artists.ArtistAdded += new ArtistEventHandler(Artists_ArtistAdded);
            this.library.Artists.ArtistRemoved += new ArtistEventHandler(Artists_ArtistRemoved);
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

        void Artists_ArtistRemoved(Artist.ArtistCollection collection, ArtistEventArgs e)
        {
            if (lookup.ContainsKey(e.Artist))
            {
                playlists.Remove(lookup[e.Artist]);
                lookup.Remove(e.Artist);
            }
            else
                throw new InvalidOperationException("Playlist was not in the LibraryPlaylist and could not be removed");
        }
        void Artists_ArtistAdded(Artist.ArtistCollection collection, ArtistEventArgs e)
        {
            ArtistPlaylist artistplaylist = new ArtistPlaylist(e.Artist);
            lookup.Add(e.Artist, artistplaylist);
            playlists.Add(artistplaylist);
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
