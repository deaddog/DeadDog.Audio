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
                return MoveNext();
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
                index--;
                if (index < 0)
                {
                    index = -2;
                    return false;
                }
                playlists[index].Reset();
                if (!playlists[index].MoveToLast())
                    return MovePrevious();
            }
            return true;
        }
        public bool MoveRandom()
        {
            Random rnd = new Random();
            List<IPlaylist<T>> temp = new List<IPlaylist<T>>(playlists);

            while (temp.Count > 0)
            {
                int i = rnd.Next(temp.Count);
                if (temp[i].MoveRandom())
                {
                    index = i;
                    return true;
                }
                else
                    temp.RemoveAt(i);
            }
            index = -2;
            return false;
        }

        public bool MoveToFirst()
        {
            if (playlists.Count == 0)
            {
                index = -2;
                return false;
            }

            index = 0;
            while (!playlists[index].MoveToFirst())
            {
                index++;
                if (index >= playlists.Count)
                {
                    index = -2;
                    return false;
                }
            }
            return true;
        }
        public bool MoveToLast()
        {
            if (playlists.Count == 0)
            {
                index = -2;
                return false;
            }

            index = playlists.Count - 1;
            while (!playlists[index].MoveToLast())
            {
                index--;
                if (index < 0)
                {
                    index = -2;
                    return false;
                }
            }
            return true;
        }
        public bool MoveToEntry(PlaylistEntry<T> entry)
        {
            for (int i = 0; i < playlists.Count; i++)
                if (playlists[i].MoveToEntry(entry))
                {
                    index = i;
                    return true;
                }

            index = -2;
            return false;
        }

        public bool Contains(PlaylistEntry<T> entry)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected void addPlaylist(IPlaylist<T> playlist)
        {
            throw new NotImplementedException();
        }

        protected void removePlaylist(IPlaylist<T> playlist)
        {
            throw new NotImplementedException();
        }
    }
}
