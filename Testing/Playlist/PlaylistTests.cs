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
    public class PlaylistTests
    {
        private Playlist<T> getPlaylist<T>(params T[] elements) where T : class
        {
            Playlist<T> playlist = new Playlist<T>();
            foreach (var n in elements)
                playlist.Add(n);
            return playlist;
        }

        private Playlist<string> getEmptyPlaylist()
        {
            return getPlaylist<string>();
        }
        private Playlist<string> getHelloWorldPlaylist(bool post = false)
        {
            var playlist = getPlaylist("hello", "world");
            if (post)
            {
                playlist.MoveToLast();
                playlist.MoveNext();
            }
            return playlist;
        }

        private void assertState<T>(Playlist<T> playlist, T expectedEntry, int expectedIndex) where T : class
        {
            Assert.AreEqual(expectedEntry, playlist.Entry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }

        private static int PreListIndex
        {
            get { return Playlist<string>.PreListIndex; }
        }

        private static int PostListIndex
        {
            get { return Playlist<string>.PostListIndex; }
        }

        [TestMethod()]
        public void PlaylistTest()
        {
            Playlist<string> playlist = new Playlist<string>();
            assertState(playlist, null, PreListIndex);
        }
    }
}
