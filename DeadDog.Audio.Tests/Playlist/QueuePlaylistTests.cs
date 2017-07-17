using DeadDog.Audio.Playlist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Playlist
{
    [TestClass]
    public class QueuePlaylistTests
    {
        [TestMethod]
        public void MoveNextTestEmpty()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };
            var queuePlaylist = new QueuePlaylist<string>(playlist);
            
            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        [TestMethod]
        public void MoveNextTestWithOne()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        [TestMethod]
        public void MoveNextTestWithFour()
        {
            var playlist = new Playlist<string>
            {
                "hello",
                "world"
            };
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");
            queuePlaylist.Enqueue("world");
            queuePlaylist.Enqueue("hello");
            queuePlaylist.Enqueue("world");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        [TestMethod]
        public void MoveNextTestComplexEmpty()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        [TestMethod]
        public void MoveNextTestComplexWithOne1()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("hello");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }
        [TestMethod]
        public void MoveNextTestComplexWithOne2()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("crazy");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }
        [TestMethod]
        public void MoveNextTestComplexWithOne3()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("wild");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }
        [TestMethod]
        public void MoveNextTestComplexWithOne4()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");

            playlist.AssertStatePre();
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }

        [TestMethod]
        public void MoveNextTestComplexWithTwo()
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
            var queuePlaylist = new QueuePlaylist<string>(playlist);

            playlist.AssertStatePre();

            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);

            queuePlaylist.Enqueue("hello");

            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("hello", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("crazy", 0);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("wild", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, true, true);
            playlist.AssertState("world", 1);
            playlist.AssertMove(queuePlaylist.MoveNext, false, true);
            playlist.AssertStatePost();
        }
    }
}
