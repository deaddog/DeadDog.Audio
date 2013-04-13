using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class PlaylistList<T> : PlaylistCollection<T>
    {
        public void Add(IPlaylist<T> playlist)
        {
            addPlaylist(playlist);
        }
        public void Remove(IPlaylist<T> playlist)
        {
            removePlaylist(playlist);
        }

        public void Move(int index, IPlaylist<T> playlist)
        {
            move(playlist, index);
        }

        public void Insert(int index, IPlaylist<T> playlist)
        {
            Add(playlist);
            Move(index, playlist);
        }

        public int Count
        {
            get { return base.count; }
        }

        public IPlaylist<T> this[int index]
        {
            get { return base.getPlaylist(index); }
        }
    }
}
