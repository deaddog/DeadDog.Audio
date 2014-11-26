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
    public class PlaylistTests : IPlaylistTester
    {
        #region Constructor

        [TestMethod()]
        public void PlaylistTest()
        {
            LoadEmptyPlaylist();
            AssertState(null, EmptyListIndex);
        }

        #endregion

        #region MoveNext

        [TestMethod()]
        public void MoveNextEmptyTest()
        {
            LoadEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveNext());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveNextTwoEntriesTest()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveNext());
            AssertState("hello", 0);

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
            LoadEmptyPlaylist();

            Assert.AreEqual(false, playlist.MovePrevious());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesDirectTest()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(false, playlist.MovePrevious());
            AssertState(null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            LoadHelloWorldPlaylist(~0);

            Assert.AreEqual(true, playlist.MovePrevious());
            AssertState("world", 1);

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
            LoadEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToLast());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToLastTwoEntriesTest()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToLast());
            AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod()]
        public void MoveToFirstEmptyTest()
        {
            LoadEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToFirst());
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToFirst());
            AssertState("hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            LoadHelloWorldPlaylist(~0);

            Assert.AreEqual(true, playlist.MoveToFirst());
            AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod()]
        public void MoveToEntryEmptyTest()
        {
            LoadEmptyPlaylist();

            Assert.AreEqual(false, playlist.MoveToEntry("hello"));
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            LoadHelloWorldPlaylist();

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            LoadHelloWorldPlaylist(1);

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            LoadHelloWorldPlaylist(~0);

            var state = PlaylistState;
            Assert.AreEqual(false, playlist.MoveToEntry("unknown"));
            AssertState(state);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("hello"));
            AssertState("hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            LoadHelloWorldPlaylist();

            Assert.AreEqual(true, playlist.MoveToEntry("world"));
            AssertState("world", 1);
        }

        #endregion

        #region Reset

        [TestMethod()]
        public void ResetEmptyTest()
        {
            LoadEmptyPlaylist();

            playlist.Reset();
            AssertState(null, EmptyListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest1()
        {
            LoadHelloWorldPlaylist();

            playlist.Reset();
            AssertState(null, PreListIndex);
        }
        [TestMethod()]
        public void ResetTwoEntriesTest2()
        {
            LoadHelloWorldPlaylist(~0);

            playlist.Reset();
            AssertState(null, PreListIndex);
        }

        #endregion
    }
}
