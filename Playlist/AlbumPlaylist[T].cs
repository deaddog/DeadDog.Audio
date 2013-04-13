﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class AlbumPlaylist : IPlaylist<Track>
        
    {
        private List<PlaylistEntry<Track>> entries;
        private int index;
        private Album album;

        public AlbumPlaylist(Album album)
        {
            this.album = album;
            entries = new List<PlaylistEntry<Track>>();
            foreach (var track in album.Tracks)
            {
                entries.Add(new PlaylistEntry<Track>(track));
            }
        }

        public PlaylistEntry<Track> CurrentEntry
        {
            get 
            {
                return index < 0 ? null : entries[index];
            }
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

        public bool MoveRandom()
        {
            if (entries.Count > 0)
            {
                Random r = new Random();
                index = r.Next(entries.Count);
                return true;
            }
            else
            {
                index = -2;
                return false;
            }
        }

        public void Reset()
        {
            index = -1;
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
            if(entries.Contains(entry))
                return true;
            else return false;
        }
    }
}
