using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    class AlbumPlaylist<T> : IPlaylist<T>
    {
        List<PlaylistEntry<T>> entries;
        int index;

        public PlaylistEntry<T> CurrentEntry
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
                currentEntry = entries[index];
                return true;
            }
            else return false;
        }

        public void Reset()
        {
            index = -1;
            currentEntry = null;
        }


    }
}
