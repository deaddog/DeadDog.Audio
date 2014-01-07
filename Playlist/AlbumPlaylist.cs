﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : Playlist<Track>, IEnumerablePlaylist<Track>
    {
        private Album album;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
        }

        public void Reset()
        {
            index = -1;
        }

        public bool MoveNext()
        {
            if (index == -2)
                return false;

            index++;
            if (index < album.Tracks.Count)
            {
                if (!TryEntryChanged(true))
                    return MoveNext();
                else
                    return true;
            }
            else
            {
                index = -2;
                return !TryEntryChanged(false);
            }
        }
        public bool MovePrevious()
        {
            if (index == -1)
                return false;

            if (index == -2)
                index = album.Tracks.Count - 1;
            else
                index--;

            if (index >= 0)
            {
                if (!TryEntryChanged(true))
                    return MovePrevious();
                else
                    return true;
            }
            else
                return !TryEntryChanged(false);

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
