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
        #region Helpers

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
        private Playlist<string> getHelloWorldPlaylist(bool post = false)
        {
            var playlist = getPlaylist("hello", "world");
            if (post)
            {
                playlist.MoveToLast();
                playlist.MoveNext();
            }
            return playlist;
        }

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
            var playlist = getHelloWorldPlaylist(true);

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
            var playlist = getHelloWorldPlaylist(true);

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
            var playlist = getHelloWorldPlaylist(true);

            Assert.AreEqual(true, playlist.MoveToFirst());
            assertState(playlist, "hello", 0);
        }

        #endregion
    }
}
