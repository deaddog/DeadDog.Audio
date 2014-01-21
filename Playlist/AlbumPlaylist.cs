using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : IPlaylist<Track>, IEnumerablePlaylist<Track>, ISeekablePlaylist<Track>
    {
        private Album album;
        private ItemPlaylist<Track> playlist;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
            this.playlist = new ItemPlaylist<Track>();

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
        private void playlist_EntryChanging(IPlaylist<Track> sender, EntryChangingEventArgs<Track> e)
        {
            if (EntryChanging != null)
                EntryChanging(this, e);
        }

        void Tracks_TrackAdded(Track.TrackCollection collection, TrackEventArgs e)
        {
            int i = entries.BinarySearch(e.Track, sort);
            if (i >= 0 && entries[i] == e.Track)
                throw new ArgumentException("A playlist cannot contain the same track twice");
            else if (i < 0)
                i = ~i;

            entries.Insert(i, e.Track);
            if (i <= index)
                index++;
        }
        void Tracks_TrackRemoved(Track.TrackCollection collection, TrackEventArgs e)
        {
            int i = entries.BinarySearch(e.Track, sort);
            if (i >= 0)
            {
                entries.RemoveAt(i);
                if (i < index)
                    index--;
                else if (i == index)
                    if (!trySettingEntry(entries[index]))
                        MoveNext();
            }
            else
                throw new ArgumentException("Playlist did not contain the track");
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
