using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadDog.Audio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests
{
    [TestClass()]
    public class PlaylistTests
    {
        #region Initialize

        private Playlist<T> getPlaylist<T>(params T[] elements) where T : class
        {
            Playlist<T> playlist = new Playlist<T>();
            foreach (var n in elements)
                playlist.Add(n);
            return playlist;
        }

        private Playlist<string> getEmptyPlaylist()
        {
            return getPlaylist<string>();
        }
        private Playlist<string> getHelloWorldPlaylist(int offset = 0)
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
            return playlist;
        }

        #endregion

        #region Assert

        private void assertState<T>(Playlist<T> playlist, T expectedEntry, int expectedIndex) where T : class
        {
            Assert.AreEqual(expectedEntry, playlist.Entry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }
        private void assertState<T>(Playlist<T> playlist, Tuple<T, int> state) where T : class
        {
            Assert.AreEqual(state.Item1, playlist.Entry);
            Assert.AreEqual(state.Item2, playlist.Index);
        }
        private Tuple<T, int> getState<T>(Playlist<T> playlist) where T : class
        {
            return Tuple.Create(playlist.Entry, playlist.Index);
        }

        private static int PreListIndex
        {
            get { return Playlist<string>.PreListIndex; }
        }
        private static int PostListIndex
        {
            get { return Playlist<string>.PostListIndex; }
        }
        private static int EmptyListIndex
        {
            get { return Playlist<string>.EmptyListIndex; }
        }

        #endregion

        #region Constructor

        [TestMethod()]
        public void PlaylistTest()
        {
            Playlist<string> playlist = new Playlist<string>();
            assertState(playlist, null, EmptyListIndex);
        }

        #endregion

        #region MoveNext

        [TestMethod()]
        public void MoveNextEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveNext());
            assertState(playlist, null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveNextTwoEntriesTest()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveNext());
            assertState(playlist, "hello", 0);

            Assert.AreEqual(true, playlist.MoveNext());
            assertState(playlist, "world", 1);

            Assert.AreEqual(false, playlist.MoveNext());
            assertState(playlist, null, PostListIndex);
        }

        #endregion
        #region MovePrevious

        [TestMethod()]
        public void MovePreviousEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            Assert.AreEqual(false, playlist.MovePrevious());
            assertState(playlist, null, EmptyListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesDirectTest()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(false, playlist.MovePrevious());
            assertState(playlist, null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            var playlist = getHelloWorldPlaylist(~0);

            Assert.AreEqual(true, playlist.MovePrevious());
            assertState(playlist, "world", 1);

            Assert.AreEqual(true, playlist.MovePrevious());
            assertState(playlist, "hello", 0);

            Assert.AreEqual(false, playlist.MovePrevious());
            assertState(playlist, null, PreListIndex);
        }

        #endregion
        #region MoveToLast

        [TestMethod()]
        public void MoveToLastEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToLast());
            assertState(playlist, null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToLastTwoEntriesTest()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToLast());
            assertState(playlist, "world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod()]
        public void MoveToFirstEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToFirst());
            assertState(playlist, null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToFirst());
            assertState(playlist, "hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            var playlist = getHelloWorldPlaylist(~0);

            Assert.AreEqual(true, playlist.MoveToFirst());
            assertState(playlist, "hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod()]
        public void MoveToEntryEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToEntry("hello"));
            assertState(playlist, null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            var playlist = getHelloWorldPlaylist();

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            var playlist = getHelloWorldPlaylist(1);

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            var playlist = getHelloWorldPlaylist(~0);

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("hello"));
            assertState(playlist, "hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            var playlist = getHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("world"));
            assertState(playlist, "world", 1);
        }

        #endregion

        #region Reset

        [TestMethod()]
        public void ResetEmptyTest()
        {
            var playlist = getEmptyPlaylist();

            playlist.Reset();
            assertState(playlist, null, EmptyListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest1()
        {
            var playlist = getHelloWorldPlaylist();

            playlist.Reset();
            assertState(playlist, null, PreListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest2()
        {
            var playlist = getHelloWorldPlaylist(~0);

            playlist.Reset();
            assertState(playlist, null, PreListIndex);
        }

        #endregion
    }
}
