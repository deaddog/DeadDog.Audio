using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio
{
    public abstract class PlaylistCollection<T> : IPlaylist<T>
    {
        private List<IPlaylist<T>> playlists;
        private int index = -1;

        public PlaylistCollection()
        {
            this.playlists = new List<IPlaylist<T>>();
        }

        public PlaylistEntry<T> CurrentEntry
        {
            get { return index < 0 ? null : playlists[index].CurrentEntry; }
        }

        public bool MoveNext()
        {
            if (index == -2)
                return false;
            else if (playlists.Count == 0)
            {
                index = -2;
                return false;
            }
            else if (index == -1)
            {
                index = 0;
                playlists[index].Reset();
            }

            if (!playlists[index].MoveNext())
            {
                index++;
                if (index >= playlists.Count)
                {
                    index = -2;
                    return false;
                }
                playlists[index].Reset();
                MoveNext();
            }
            return true;
        }
        public bool MovePrevious()
        {
            if (index == -2)
                return false;

            if (playlists.Count == 0 || index == -1)
            {
                index = -2;
                return false;
            }

            if (!playlists[index].MovePrevious())
            {
                do
                {
                    index--;
                    if (index < 0)
                    {
                        index = -2;
                        return false;
                    }
                } while (!playlists[index].MoveToLast());
            }
            return true;
        }
        public bool MoveRandom()
        {
            Random rnd = new Random();
            List<IPlaylist<T>> temp = new List<IPlaylist<T>>(playlists);

            int i = rnd.Next(temp.Count);
            if (!temp[i].MoveRandom())
            {
                temp.RemoveAt(i);
            }
            return false;
        }

        public bool MoveToFirst()
        {
            throw new NotImplementedException();
        }
        public bool MoveToLast()
        {
            throw new NotImplementedException();
        }
        public bool MoveToEntry(PlaylistEntry<T> entry)
        {
            throw new NotImplementedException();
        }

        public bool Contains(PlaylistEntry<T> entry)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
