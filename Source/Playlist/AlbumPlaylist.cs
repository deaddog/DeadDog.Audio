using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : IPlaylist<Track>
    {
        private Album album;
        private Playlist<Track> playlist;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
            this.playlist = new Playlist<Track>();

            this.playlist.EntryChanged += playlist_EntryChanged;
            this.playlist.EntryChanging += playlist_EntryChanging;

            foreach (var track in album.Tracks)
                playlist.Add(track);

            this.album.Tracks.TrackAdded += new TrackEventHandler(Tracks_TrackAdded);
            this.album.Tracks.TrackRemoved += new TrackEventHandler(Tracks_TrackRemoved);
        }

        private void playlist_EntryChanged(object sender, EventArgs e)
        {
            if (EntryChanged != null)
                EntryChanged(this, e);
        }
        private void playlist_EntryChanging(IPlayable<Track> sender, EntryChangingEventArgs<Track> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
        }

        void Tracks_TrackAdded(TrackCollection collection, TrackEventArgs e)
        {
            int i = collection.IndexOf(e.Track);
            playlist.Insert(i, e.Track);
        }
        void Tracks_TrackRemoved(TrackCollection collection, TrackEventArgs e)
        {
            playlist.Remove(e.Track);
        }

        public Track Entry
        {
            get { return playlist.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<Track> EntryChanging;

        public void Reset()
        {
            playlist.Reset();
        }

        public bool MoveNext()
        {
            return playlist.MoveNext();
        }
        public bool MovePrevious()
        {
            return playlist.MovePrevious();
        }

        public bool MoveToFirst()
        {
            return playlist.MoveToFirst();
        }
        public bool MoveToLast()
        {
            return playlist.MoveToLast();
        }

        public bool MoveToEntry(Track entry)
        {
            return playlist.MoveToEntry(entry);
        }
        public bool Contains(Track entry)
        {
            return playlist.Contains(entry);
        }

        #region IEnumerable<PlaylistEntry<Track>> Members

        IEnumerator<Track> IEnumerable<Track>.GetEnumerator()
        {
            return (playlist as IEnumerable<Track>).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (playlist as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
