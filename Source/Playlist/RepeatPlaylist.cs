using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

﻿namespace DeadDog.Audio.Playlist
{
    public class RepeatPlaylist<T> : IPlayable<T>
    {
        private IPlaylist<T> playlist;
        private bool repeat;

        public RepeatPlaylist(IPlaylist<T> playlist, bool repeat)
        {
            this.playlist = playlist;
            this.Repeat = repeat;
        }

        public T Entry
        {
            get { return playlist.Entry; }
        }
        public bool Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

        public event EventHandler EntryChanged
        {
            add { playlist.EntryChanged += value; }
            remove { playlist.EntryChanged -= value; }
        }

        public event EntryChangingEventHandler<T> EntryChanging
        {
            add { playlist.EntryChanging += value; }
            remove { playlist.EntryChanging -= value; }
        }

        public bool MoveNext()
        {
            if (playlist.MoveNext())
                return true;
            else if (repeat)
            {
                playlist.Reset();
                return playlist.MoveNext();
            }
            else
                return false;
        }
    }
}
