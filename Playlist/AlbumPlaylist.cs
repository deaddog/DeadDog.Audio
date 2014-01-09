using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : Playlist<Track>, IEnumerablePlaylist<Track>
    {
        /// <summary>
        /// Determines if the playlist, when pointing to null is pointing in front of the tracklist.
        /// </summary>
        private bool initial;
        private Album album;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
        }

        public void Reset()
        {
            if (Entry != null)
                Entry = null;
            initial = true;
        }

        public bool MoveNext()
        {
            int index = Entry == null ? -1 : album.Tracks.IndexOf(Entry);
            int nextIndex = index == -1 ? (initial ? 0 : -1) : index + 1;

            if (index == -1 && nextIndex == -1)
                return false;

            if (nextIndex < album.Tracks.Count)
            {
                if (!trySettingEntry(album.Tracks[nextIndex]))
                    return MoveNext();
                else
                    return true;
            }
            else
            {
                Entry = null;
                return false;
            }
        }
        public bool MovePrevious()
        {
            int index = Entry == null ? -1 : album.Tracks.IndexOf(Entry);
            int nextIndex = index == -1 ? (initial ? -1 : album.Tracks.Count - 1) : index - 1;

            if (index == -1 && nextIndex == -1)
                return false;

            if (nextIndex >= 0)
            {
                if (!trySettingEntry(album.Tracks[nextIndex]))
                    return MovePrevious();
                else
                    return true;
            }
            else
            {
                Entry = null;
                initial = true;
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

        public bool MoveToEntry(PlaylistEntry<Track> entry)
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

        public bool Contains(PlaylistEntry<Track> entry)
        {
            if (entries.Contains(entry))
                return true;
            else return false;
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
