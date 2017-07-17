using DeadDog.Audio.Playlist;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DeadDog.Audio.Tests.Playlist
{
    [TestClass]
    public class PlaylistCollectionTests
    {
        #region Constructor

        [TestMethod]
        public void PlaylistCollectionTest()
        {
            var playlist = new PlaylistCollection<string>();

            playlist.AssertEmpty();
        }

        #endregion

        #region MoveNext

        [TestMethod]
        public void MoveNextEmptyTest()
        {
            var playlist = new PlaylistCollection<string>();

            playlist.AssertMove(playlist.MoveNext, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveNextTwoEntriesTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove(playlist.MoveNext, true, true);
            playlist.AssertState("hello", 0);

            playlist.AssertMove(playlist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);

            playlist.AssertMove(playlist.MoveNext, true, true);
            playlist.AssertState("wild", 1);

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
            var playlist = new PlaylistCollection<string>();

            playlist.AssertMove(playlist.MovePrevious, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MovePreviousTwoEntriesDirectTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove(playlist.MovePrevious, false, false);
            playlist.AssertStatePre();
        }

        [TestMethod]
        public void MovePreviousTwoEntriesTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.MoveToLast();
            playlist.MoveNext();
            playlist.AssertStatePost();

            playlist.AssertMove(playlist.MovePrevious, true, true);
            playlist.AssertState("world", 1);

            playlist.AssertMove(playlist.MovePrevious, true, true);
            playlist.AssertState("wild", 1);

            playlist.AssertMove(playlist.MovePrevious, true, true);
            playlist.AssertState("crazy", 0);

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
            var playlist = new PlaylistCollection<string>();

            playlist.AssertMove(playlist.MoveToLast, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveToLastTwoEntriesTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove(playlist.MoveToLast, true, true);
            playlist.AssertState("world", 1);
        }

        #endregion
        #region MoveToFirst

        [TestMethod]
        public void MoveToFirstEmptyTest()
        {
            var playlist = new PlaylistCollection<string>();

            playlist.AssertMove(playlist.MoveToFirst, false, false);
            playlist.AssertEmpty();
        }

        [TestMethod]
        public void MoveToFirstTwoEntriesDirectTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove(playlist.MoveToFirst, true, true);
            playlist.AssertState("hello", 0);
        }

        [TestMethod]
        public void MoveToFirstTwoEntriesTest()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.MoveToLast();
            playlist.MoveNext();
            playlist.AssertStatePost();

            playlist.AssertMove(playlist.MoveToFirst, true, true);
            playlist.AssertState("hello", 0);
        }

        #endregion
        #region MoveToEntry

        [TestMethod]
        public void MoveToEntryEmptyTest()
        {
            var playlist = new PlaylistCollection<string>();

            playlist.AssertMove("hello", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest1()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest2()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.MoveNext();
            playlist.AssertState("hello", 0);

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesUnknownKeyTest3()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.MoveToLast();
            playlist.MoveNext();
            playlist.AssertStatePost();

            playlist.AssertMove("unknown", false, false);
        }

        [TestMethod]
        public void MoveToEntryTwoEntriesWithKeyTest1()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove("hello", true, true);
            playlist.AssertState("hello", 0);
        }
        [TestMethod]
        public void MoveToEntryTwoEntriesWithKeyTest2()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.AssertMove("wild", true, true);
            playlist.AssertState("wild", 1);
        }

        #endregion

        #region Reset

        [TestMethod]
        public void ResetEmptyTest()
        {
            var playlist = new PlaylistCollection<string>();

            playlist.Reset();
            playlist.AssertEmpty();
        }
        [TestMethod]
        public void ResetTwoEntriesTest1()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.Reset();
            playlist.AssertStatePre();
        }
        [TestMethod]
        public void ResetTwoEntriesTest2()
        {
            var playlist = new PlaylistCollection<string>()
            {
                new Playlist<string>
                {
                    "hello",
                    "crazy"
                },
                new Playlist<string>
                {
                    "wild",
                    "world"
                }
            };

            playlist.MoveToLast();
            playlist.MoveNext();
            playlist.AssertStatePost();

            playlist.Reset();
            playlist.AssertStatePre();
        }

        #endregion
    }
}
