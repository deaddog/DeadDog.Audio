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
    public class PlaylistCollectionTests : IPlaylistTester
    {
        #region Constructor

        [TestMethod()]
        public void PlaylistCollectionTest()
        {
            LoadEmptyPlaylistCollection();
            AssertState(null, EmptyListIndex);
        }

        #endregion

        #region MoveNext

        [TestMethod()]
        public void MoveNextEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            Assert.AreEqual(false, playlist.MoveNext());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveNextTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(true, playlist.MoveNext());
            AssertState("hello", 0);

            Assert.AreEqual(true, playlist.MoveNext());
            AssertState("crazy", 0);

            Assert.AreEqual(true, playlist.MoveNext());
            AssertState("wild", 1);

            Assert.AreEqual(true, playlist.MoveNext());
            AssertState("world", 1);

            Assert.AreEqual(false, playlist.MoveNext());
            AssertState(null, PostListIndex);
        }

        #endregion
        #region MovePrevious

        [TestMethod()]
        public void MovePreviousEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            Assert.AreEqual(false, playlist.MovePrevious());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesDirectTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(false, playlist.MovePrevious());
            AssertState(null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            Assert.AreEqual(true, playlist.MovePrevious());
            AssertState("world", 1);

            Assert.AreEqual(true, playlist.MovePrevious());
            AssertState("wild", 1);

            Assert.AreEqual(true, playlist.MovePrevious());
            AssertState("crazy", 0);

            Assert.AreEqual(true, playlist.MovePrevious());
            AssertState("hello", 0);

            Assert.AreEqual(false, playlist.MovePrevious());
            AssertState(null, PreListIndex);
        }

        #endregion
        #region MoveToLast

        [TestMethod()]
        public void MoveToLastEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            Assert.AreEqual(false, playlist.MoveToLast());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToLastTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(true, playlist.MoveToLast());
            AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod()]
        public void MoveToFirstEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            Assert.AreEqual(false, playlist.MoveToFirst());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(true, playlist.MoveToFirst());
            AssertState("hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            Assert.AreEqual(true, playlist.MoveToFirst());
            AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod()]
        public void MoveToEntryEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            Assert.AreEqual(false, playlist.MoveToEntry("hello"));
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(1);

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(true, playlist.MoveToEntry("hello"));
            AssertState("hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            Assert.AreEqual(true, playlist.MoveToEntry("wild"));
            AssertState("wild", 1);
        }

        #endregion

        #region Reset

        [TestMethod()]
        public void ResetEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            playlist.Reset();
            AssertState(null, EmptyListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            playlist.Reset();
            AssertState(null, PreListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            playlist.Reset();
            AssertState(null, PreListIndex);
        }

        #endregion
    }
}
