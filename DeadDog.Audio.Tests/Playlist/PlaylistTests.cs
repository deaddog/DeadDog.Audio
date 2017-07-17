using DeadDog.Audio.Playlist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Playlist
{
    [TestClass]
    public class PlaylistTests
    {
        #region Constructor

        [TestMethod]
        public void PlaylistTest()
        {
            var playlist = new Playlist<string>();
            playlist.AssertEmpty();
        }

        #endregion

        #region MoveEntry

        [TestMethod]
        public void MoveEntryMoveActiveForwardTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("are", 5);
            playlist.AssertState("are", 5);

            playlist.AssertAllForwards("today");
        }

        [TestMethod]
        public void MoveEntryMoveActiveBackwardTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("are", 1);
            playlist.AssertState("are", 1);

            playlist.AssertAllBackwards("hello");
        }

        [TestMethod]
        public void MoveEntryMoveActiveNowhereTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("are", 3);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "doing", "today");
        }

        [TestMethod]
        public void MoveEntryMoveActiveLastTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("are", 6);
            playlist.AssertState("are", 6);

            playlist.AssertAllForwards();
        }

        [TestMethod]
        public void MoveEntryMoveActiveFirstTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("are", 0);
            playlist.AssertState("are", 0);

            playlist.AssertAllBackwards();
        }


        [TestMethod]
        public void MoveEntryMovePreviousForwardTest1()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("world", 2);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "doing", "today");
        }

        [TestMethod]
        public void MoveEntryMovePreviousForwardTest2()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("world", 5);
            playlist.AssertState("are", 2);

            playlist.AssertAllForwards("you", "doing", "world", "today");
        }

        [TestMethod]
        public void MoveEntryMovePreviousBackwardTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("how", 1);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "doing", "today");
        }

        [TestMethod]
        public void MoveEntryMovePreviousNowhereTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("how", 1);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "doing", "today");
        }

        [TestMethod]
        public void MoveEntryMovePreviousLastTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("how", 6);
            playlist.AssertState("are", 2);

            playlist.AssertAllForwards("you", "doing", "today", "how");
        }

        [TestMethod]
        public void MoveEntryMovePreviousFirstTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("how", 0);
            playlist.AssertState("are", 3);

            playlist.AssertAllBackwards("world", "hello", "how");
        }


        [TestMethod]
        public void MoveEntryMoveNextForwardTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("you", 5);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("doing", "you", "today");
        }

        [TestMethod]
        public void MoveEntryMoveNextBackwardTest1()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("doing", 4);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("doing", "you", "today");
        }

        [TestMethod]
        public void MoveEntryMoveNextBackwardTest2()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("doing", 1);
            playlist.AssertState("are", 4);

            playlist.AssertAllForwards("you", "today");
        }

        [TestMethod]
        public void MoveEntryMoveNextNowhereTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("doing", 5);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "doing", "today");
        }

        [TestMethod]
        public void MoveEntryMoveNextLastTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("doing", 6);
            playlist.AssertState("are", 3);

            playlist.AssertAllForwards("you", "today", "doing");
        }

        [TestMethod]
        public void MoveEntryMoveNextFirstTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world",
                "how",
                "are",
                "you",
                "doing",
                "today"
            };

            playlist.Index = 3;
            playlist.AssertState("are", 3);

            playlist.AssertMoveEntry("doing", 0);
            playlist.AssertState("are", 4);

            playlist.AssertAllBackwards("how", "world", "hello", "doing");
        }

        #endregion

        #region MoveNext

        [TestMethod]
        public void MoveNextEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.AssertMove(playlist.MoveNext, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveNextTwoEntriesTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove(playlist.MoveNext, true, true);
            playlist.AssertState("hello", 0);

            playlist.AssertMove(playlist.MoveNext, true, true);
            playlist.AssertState("world", 1);

            playlist.AssertMove(playlist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        #endregion
        #region MovePrevious

        [TestMethod]
        public void MovePreviousEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.AssertMove(playlist.MovePrevious, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MovePreviousTwoEntriesDirectTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove(playlist.MovePrevious, false, false);
            playlist.AssertStatePre();
        }

        [TestMethod]
        public void MovePreviousTwoEntriesTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.Index = Playlist<string>.PostListIndex;
            playlist.AssertStatePost();

            playlist.AssertMove(playlist.MovePrevious, true, true);
            playlist.AssertState("world", 1);

            playlist.AssertMove(playlist.MovePrevious, true, true);
            playlist.AssertState("hello", 0);

            playlist.AssertMove(playlist.MovePrevious, false, true);
            playlist.AssertStatePre();
        }

        #endregion
        #region MoveToLast

        [TestMethod]
        public void MoveToLastEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.AssertMove(playlist.MoveToLast, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveToLastTwoEntriesTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove(playlist.MoveToLast, true, true);
            playlist.AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod]
        public void MoveToFirstEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.AssertMove(playlist.MoveToFirst, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove(playlist.MoveToFirst, true, true);
            playlist.AssertState("hello", 0);
        }

        [TestMethod]
        public void MoveToFirstTwoEntriesTest()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.Index = Playlist<string>.PostListIndex;
            playlist.AssertStatePost();

            playlist.AssertMove(playlist.MoveToFirst, true, true);
            playlist.AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod]
        public void MoveToEntryEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.AssertMove("hello", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };
            playlist.Index = 0;
            playlist.AssertState("hello", 0);

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.Index = Playlist<string>.PostListIndex;
            playlist.AssertStatePost();

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove("hello", true, true);
            playlist.AssertState("hello", 0);
        }
        [TestMethod]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.AssertMove("world", true, true);
            playlist.AssertState("world", 1);
        }

        #endregion

        #region Reset

        [TestMethod]
        public void ResetEmptyTest()
        {
            var playlist = new Playlist<string>();

            playlist.Reset();
            playlist.AssertEmpty();
        }
        [TestMethod]
        public void ResetTwoEntriesTest1()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.Reset();
            playlist.AssertStatePre();
        }
        [TestMethod]
        public void ResetTwoEntriesTest2()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };

            playlist.Index = Playlist<string>.PostListIndex;
            playlist.AssertStatePost();

            playlist.Reset();
            playlist.AssertStatePre();
        }

        #endregion
    }
}
