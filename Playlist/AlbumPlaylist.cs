using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : Playlist<Track>, IEnumerablePlaylist<Track>
    {
        private List<Track> entries;
        private int index;
        private Album album;
        private Comparison<Track> sort;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
            entries = new List<Track>();

            foreach (var track in album.Tracks)
                entries.Add(track);

            this.album.Tracks.TrackAdded += new TrackEventHandler(Tracks_TrackAdded);
            this.album.Tracks.TrackRemoved += new TrackEventHandler(Tracks_TrackRemoved);

            SetSortMethod(DefaultSort);
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

        public bool MoveNext()
        {
            if (index == -2)
                return false;

            index++;
            if (index < entries.Count)
                return true;
            else
            {
                index = -2;
                return false;
            }
        }

        public bool MovePrevious()
        {
            if (index == -2)
                return false;

            index--;
            if (index < 0)
                return true;
            else
            {
                index = -2;
                return false;
            }

        }

        public bool MoveToFirst()
        {
            if (entries.Count == 0)
                return false;
            else
            {
                index = 0;
                return true;
            }

        }

        public bool MoveToLast()
        {
            if (entries.Count == 0)
            {
                index = -2;
                return false;
            }
            else
            {
                index = entries.Count;
                return true;
            }
        }

        public bool MoveToEntry(Track entry)
        {
            if (entries.Contains(entry))
            {
                index = entries.IndexOf(entry);
                return true;
            }
            else
            {
                index = -2;
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

        IEnumerator<PlaylistEntry<Track>> IEnumerable<PlaylistEntry<Track>>.GetEnumerator()
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
