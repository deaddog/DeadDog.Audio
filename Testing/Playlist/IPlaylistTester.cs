﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadDog.Audio.Tests
{
    public abstract class IPlaylistTester
    {
        private Playlist<T> getPlaylist<T>(params T[] elements) where T : class
        {
            Playlist<T> playlist = new Playlist<T>();
            foreach (var n in elements)
                playlist.Add(n);
            return playlist;
        }

        protected void LoadEmptyPlaylist()
        {
            this._playlist = getPlaylist<string>();
        }
        protected void LoadHelloWorldPlaylist(int offset = 0)
        {
            var playlist = getPlaylist("hello", "world");
            if (offset < 0)
            {
                playlist.MoveToLast();
                playlist.MoveNext();

                offset = ~offset;
                while (offset-- > 0)
                    playlist.MovePrevious();
            }
            else
            {
                while (offset-- > 0)
                    playlist.MoveNext();
            }
            this._playlist = playlist;
        }

        private IPlaylist<string> _playlist;
        protected IPlaylist<string> playlist { get { return _playlist; } }

        [TestInitialize]
        public void Initialize()
        {
            _playlist = null;
        }

        protected void AssertState(string expectedEntry)
        {
            Assert.AreEqual(expectedEntry, _playlist.Entry);
        }
        protected void AssertState(string expectedEntry, int expectedIndex)
        {
            AssertState(expectedEntry);

            if (_playlist is Playlist<string>)
                Assert.AreEqual(expectedIndex, (_playlist as Playlist<string>).Index);
            else if (_playlist is PlaylistCollection<string>)
                Assert.AreEqual(expectedIndex, (_playlist as PlaylistCollection<string>).Index);
            else
                throw new InvalidOperationException("Unable to assert the index on a playlist that is not Playlist or PlaylistCollection.");
        }
        protected void AssertState(Tuple<string, int> state)
        {
            Assert.AreEqual(state.Item1, _playlist.Entry);

            if (_playlist is Playlist<string>)
                Assert.AreEqual(state.Item2, (_playlist as Playlist<string>).Index);
            else if (_playlist is PlaylistCollection<string>)
                Assert.AreEqual(state.Item2, (_playlist as PlaylistCollection<string>).Index);
            else
                throw new InvalidOperationException("Unable to assert the index on a playlist that is not Playlist or PlaylistCollection.");
        }
        protected Tuple<string, int> PlaylistState
        {
            get
            {
                if (_playlist is Playlist<string>)
                    return Tuple.Create(playlist.Entry, (_playlist as Playlist<string>).Index);
                else if (_playlist is PlaylistCollection<string>)
                    return Tuple.Create(playlist.Entry, (_playlist as PlaylistCollection<string>).Index);
                else
                    throw new InvalidOperationException("Unable to get the index on a playlist that is not Playlist or PlaylistCollection.");
            }
        }

        protected static int PreListIndex
        {
            get { return Playlist<string>.PreListIndex; }
        }
        protected static int PostListIndex
        {
            get { return Playlist<string>.PostListIndex; }
        }
        protected static int EmptyListIndex
        {
            get { return Playlist<string>.EmptyListIndex; }
        }
    }
}
