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

            AssertMove(playlist.MoveNext, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveNextTwoEntriesTest()
        {
            LoadHelloWorldPlaylist();

            AssertMove(playlist.MoveNext, true, true);
            AssertState("hello", 0);

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
            LoadEmptyPlaylist();

            AssertMove(playlist.MovePrevious, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesDirectTest()
        {
            LoadHelloWorldPlaylist();

            AssertMove(playlist.MovePrevious, false, false);
            AssertState(null, PreListIndex);
        }

        [TestMethod()]
        public void MovePreviousTwoEntriesTest()
        {
            LoadHelloWorldPlaylist(~0);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("world", 1);

            AssertMove(playlist.MovePrevious, true, true);
            AssertState("hello", 0);

            AssertMove(playlist.MovePrevious, false, true);
            AssertState(null, PreListIndex);
        }

        #endregion
        #region MoveToLast

        [TestMethod()]
        public void MoveToLastEmptyTest()
        {
            LoadEmptyPlaylist();

            AssertMove(playlist.MoveToLast, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToLastTwoEntriesTest()
        {
            LoadHelloWorldPlaylist();

            AssertMove(playlist.MoveToLast, true, true);
            AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod()]
        public void MoveToFirstEmptyTest()
        {
            LoadEmptyPlaylist();

            AssertMove(playlist.MoveToFirst, false, false);
            AssertState(null, EmptyListIndex);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            LoadHelloWorldPlaylist();

            AssertMove(playlist.MoveToFirst, true, true);
            AssertState("hello", 0);
        }

        [TestMethod()]
        public void MoveToFirstTwoEntriesTest()
        {
            LoadHelloWorldPlaylist(~0);

            AssertMove(playlist.MoveToFirst, true, true);
            AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod()]
        public void MoveToEntryEmptyTest()
        {
            LoadEmptyPlaylist();

            AssertMove("hello", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            LoadHelloWorldPlaylist();

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            LoadHelloWorldPlaylist(1);

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            LoadHelloWorldPlaylist(~0);

            AssertMove("unknown", false, false);
        }

        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            LoadHelloWorldPlaylist();

            AssertMove("hello", true, true);
            AssertState("hello", 0);
        }
        [TestMethod()]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            LoadHelloWorldPlaylist();

            AssertMove("world", true, true);
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
