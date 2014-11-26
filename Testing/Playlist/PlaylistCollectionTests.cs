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

            AssertMove(playlist.MoveNext, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveNextTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove(playlist.MoveNext, true, true);
            AssertState("hello", 0);

            AssertMove(playlist.MoveNext, true, true);
            AssertState("crazy", 0);

            AssertMove(playlist.MoveNext, true, true);
            AssertState("wild", 1);

            AssertMove(playlist.MoveNext, true, true);
            AssertState("world", 1);

            AssertMove(playlist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        #endregion
        #region MovePrevious

        [TestMethod()]
        public void MovePreviousEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            AssertMove(playlist.MovePrevious, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesDirectTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove(playlist.MovePrevious, false, false);
            AssertState(null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("world", 1);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("wild", 1);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("crazy", 0);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("hello", 0);

            AssertMove(playlist.MovePrevious, true, false);
            AssertState(null, PreListIndex);
        }

        #endregion
        #region MoveToLast

        [TestMethod()]
        public void MoveToLastEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            AssertMove(playlist.MoveToLast, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToLastTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove(playlist.MoveToLast, true, true);
            AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod()]
        public void MoveToFirstEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            AssertMove(playlist.MoveToFirst, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove(playlist.MoveToFirst, true, true);
            AssertState("hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            AssertMove(playlist.MoveToFirst, true, true);
            AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod()]
        public void MoveToEntryEmptyTest()
        {
            LoadEmptyPlaylistCollection();

            AssertMove("hello", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(1);

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            LoadHelloCrazyWildWorldPlaylistCollection(~0);

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove("hello", true, true);
            AssertState("hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            AssertMove("wild", true, true);
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
