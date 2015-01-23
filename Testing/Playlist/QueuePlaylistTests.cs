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
    public class QueuePlaylistTests : IPlaylistTester
    {
        [TestMethod()]
        public void MoveNextTestEmpty()
        {
            LoadHelloWorldPlaylist();

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("world");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("world");
            queue.Enqueue("world");
            queue.Enqueue("hello");
            queue.Enqueue("world");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("hello");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("crazy");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("wild");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            queue.Enqueue("world");

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

            Queue<string> queue = new Queue<string>();
            QueuePlaylist<string> queuePlaylist = new QueuePlaylist<string>(queue, playlist);

            AssertState(null, PreListIndex);
            
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("hello", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("crazy", 0);
            AssertMove(queuePlaylist.MoveNext, true, true);
            AssertState("wild", 1);

            queue.Enqueue("hello");

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
