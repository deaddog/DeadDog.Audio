using DeadDog.Audio.Playlist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Playlist
{
    [TestClass()]
    public class QueuePlaylistTests : IPlaylistTester
    {
        [TestMethod()]
        public void MoveNextTestEmpty()
        {
            LoadHelloWorldPlaylist();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        [TestMethod()]
        public void MoveNextTestWithOne()
        {
            LoadHelloWorldPlaylist();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        [TestMethod()]
        public void MoveNextTestWithFour()
        {
            LoadHelloWorldPlaylist();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");
            queuePlaylist.Enqueue("world");
            queuePlaylist.Enqueue("hello");
            queuePlaylist.Enqueue("world");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        [TestMethod()]
        public void MoveNextTestComplexEmpty()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        [TestMethod()]
        public void MoveNextTestComplexWithOne1()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("hello");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }
        [TestMethod()]
        public void MoveNextTestComplexWithOne2()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("crazy");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }
        [TestMethod()]
        public void MoveNextTestComplexWithOne3()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("wild");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }
        [TestMethod()]
        public void MoveNextTestComplexWithOne4()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            queuePlaylist.Enqueue("world");

            AssertState(null, PreListIndex);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }

        [TestMethod()]
        public void MoveNextTestComplexWithTwo()
        {
            LoadHelloCrazyWildWorldPlaylistCollection();

            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(playlist);

            AssertState(null, PreListIndex);

            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);

            queuePlaylist.Enqueue("hello");

            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("world", 1);
            AssertMove(queuePlaylist.MoveNext, false, true);
            AssertState(null, PostListIndex);
        }
    }
}
