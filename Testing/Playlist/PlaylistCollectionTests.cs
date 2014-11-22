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
    public class PlaylistCollectionTests
    {
        #region Helpers

        private PlaylistCollection<T> getPlaylist<T>(params IPlaylist<T>[] playlists) where T : class
        {
            PlaylistCollection<T> playlist = new PlaylistCollection<T>();
            foreach (var n in playlists)
                playlist.Add(n);
            return playlist;
        }

        private Playlist<T> getPlaylist<T>(params T[] elements) where T : class
        {
            Playlist<T> playlist = new Playlist<T>();
            foreach (var n in elements)
                playlist.Add(n);
            return playlist;
        }
        private PlaylistCollection<string> getEmptyPlaylist()
        {
            return getPlaylist<string>();
        }
        private PlaylistCollection<string> getHelloCrazyWildWorldPlaylist(int offset = 0)
        {
            var playlist = getPlaylist<string>(getPlaylist("hello", "crazy"), getPlaylist("wild", "world"));
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

        private void assertState<T>(PlaylistCollection<T> playlist, T expectedEntry, int expectedIndex) where T : class
        {
            Assert.AreEqual(expectedEntry, playlist.Entry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }
        private void assertState<T>(PlaylistCollection<T> playlist, Tuple<T, int> state) where T : class
        {
            Assert.AreEqual(state.Item1, playlist.Entry);
            Assert.AreEqual(state.Item2, playlist.Index);
        }
        private Tuple<T, int> getState<T>(PlaylistCollection<T> playlist) where T : class
        {
            return Tuple.Create(playlist.Entry, playlist.Index);
        }

        private static int PreListIndex
        {
            get { return PlaylistCollection<string>.PreListIndex; }
        }

        private static int PostListIndex
        {
            get { return PlaylistCollection<string>.PostListIndex; }
        }

        private static int EmptyListIndex
        {
            get { return PlaylistCollection<string>.EmptyListIndex; }
        }

        #endregion

        #region Constructor

        [TestMethod()]
        public void PlaylistCollectionTest()
        {
            PlaylistCollection<string> playlist = new PlaylistCollection<string>();
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
            var playlist = getHelloCrazyWildWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveNext());
            assertState(playlist, "hello", 0);

            Assert.AreEqual(true, playlist.MoveNext());
            assertState(playlist, "crazy", 0);

            Assert.AreEqual(true, playlist.MoveNext());
            assertState(playlist, "wild", 1);

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
            var playlist = getHelloCrazyWildWorldPlaylist();

            Assert.AreEqual(false, playlist.MovePrevious());
            assertState(playlist, null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            var playlist = getHelloCrazyWildWorldPlaylist(~0);

            Assert.AreEqual(true, playlist.MovePrevious());
            assertState(playlist, "world", 1);

            Assert.AreEqual(true, playlist.MovePrevious());
            assertState(playlist, "wild", 1);

            Assert.AreEqual(true, playlist.MovePrevious());
            assertState(playlist, "crazy", 0);

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
            var playlist = getHelloCrazyWildWorldPlaylist();

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
            var playlist = getHelloCrazyWildWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToFirst());
            assertState(playlist, "hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            var playlist = getHelloCrazyWildWorldPlaylist(~0);

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
            var playlist = getHelloCrazyWildWorldPlaylist();

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            var playlist = getHelloCrazyWildWorldPlaylist(1);

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            var playlist = getHelloCrazyWildWorldPlaylist(~0);

            var state = getState(playlist);
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            assertState(playlist, state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            var playlist = getHelloCrazyWildWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("hello"));
            assertState(playlist, "hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            var playlist = getHelloCrazyWildWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("wild"));
            assertState(playlist, "wild", 1);
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
            var playlist = getHelloCrazyWildWorldPlaylist();

            playlist.Reset();
            assertState(playlist, null, PreListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest2()
        {
            var playlist = getHelloCrazyWildWorldPlaylist(~0);

            playlist.Reset();
            assertState(playlist, null, PreListIndex);
        }

        #endregion
    }
}
