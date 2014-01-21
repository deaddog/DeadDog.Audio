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

            SetSortMethod(DefaultSort);
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

        public override void Reset()
        {
            if (Entry != null)
                Entry = null;
            index = -1;
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

        public override bool MoveNext()
        {
            if (index == -2)
                return false;

            index++;
            if (index < entries.Count)
            {
                if (trySettingEntry(entries[index]))
                    return true;
                else
                    return MoveNext();
            }
            else
            {
                index = -2;
                if (Entry != null)
                    Entry = null;
                return false;
            }
        }
        public bool MovePrevious()
        {
            if (index == -1)
                return false;

            index = index == -2 ? entries.Count - 1 : index - 1;
            if (index >= 0)
            {
                if (trySettingEntry(entries[index]))
                    return true;
                else
                    return MovePrevious();
            }
            else
            {
                index = -1;
                if (Entry != null)
                    Entry = null;
                return false;
            }

        }

        public bool MoveToFirst()
        {
            if (entries.Count == 0)
                return false;
            else if (index == 0)
                return true;
            else
            {
                index = 0;
                Entry = entries[index];
                return true;
            }

        }
        public bool MoveToLast()
        {
            if (entries.Count == 0)
                return false;
            else if (index == entries.Count - 1)
                return true;
            else
            {
                index = entries.Count;
                Entry = entries[index];
                return true;
            }
        }

        public bool MoveToEntry(Track entry)
        {
            if (entries.Contains(entry))
            {
                index = entries.IndexOf(entry);
                if (Entry != entry)
                    Entry = entry;
                return true;
            }
            else
            {
                index = -2;
                if (Entry != null)
                    Entry = null;
                return false;
            }

        }

        public bool Contains(Track entry)
        {
            return entries.Contains(entry);
        }

        public void SetSortMethod(Comparison<Track> sort)
        {
            if (sort == null)
                throw new ArgumentNullException("Sortmethod cannot be null. Consider setting to the DefaultSort Method");
            else
            {
                this.sort = sort;
                if (index >= 0)
                {
                    Track track = entries[index];
                    entries.Sort(sort);
                    index = entries.IndexOf(track);
                }
                else
                    entries.Sort(sort);
            }
        }

        public static Comparison<Track> DefaultSort
        {
            get { return Compare; }
        }

        private static int Compare(Track element1, Track element2)
        {
            int? v1 = element1.Tracknumber, v2 = element2.Tracknumber;
            if (v1.HasValue)
                return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
            else
                return v2.HasValue ? -1 : 0;
        }

        #region IEnumerable<PlaylistEntry<Track>> Members

        IEnumerator<Track> IEnumerable<Track>.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        #endregion
    }
}
