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
    public class PlaylistCollectionTests
    {
        #region Helpers

        private PlaylistCollection<T> getPlaylist<T>(params IPlaylist<T>[] playlists) where T : class
        {
            PlaylistCollection<T> playlist = new PlaylistCollection<T>();
            foreach (var n in playlists)
                playlist.Add(n);
            return playlist;
        }

        private Playlist<T> getPlaylist<T>(params T[] elements) where T : class
        {
            Playlist<T> playlist = new Playlist<T>();
            foreach (var n in elements)
                playlist.Add(n);
            return playlist;
        }
        private PlaylistCollection<string> getEmptyPlaylist()
        {
            return getPlaylist<string>();
        }

        private void assertState<T>(PlaylistCollection<T> playlist, T expectedEntry, int expectedIndex) where T : class
        {
            Assert.AreEqual(expectedEntry, playlist.Entry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }
        private void assertState<T>(PlaylistCollection<T> playlist, Tuple<T, int> state) where T : class
        {
            Assert.AreEqual(state.Item1, playlist.Entry);
            Assert.AreEqual(state.Item2, playlist.Index);
        }
        private Tuple<T, int> getState<T>(PlaylistCollection<T> playlist) where T : class
        {
            return Tuple.Create(playlist.Entry, playlist.Index);
        }

        private static int PreListIndex
        {
            get { return PlaylistCollection<string>.PreListIndex; }
        }

        private static int PostListIndex
        {
            get { return PlaylistCollection<string>.PostListIndex; }
        }

        private static int EmptyListIndex
        {
            get { return PlaylistCollection<string>.EmptyListIndex; }
        }

        #endregion

        #region Constructor

        [TestMethod()]
        public void PlaylistCollectionTest()
        {
            PlaylistCollection<string> playlist = new PlaylistCollection<string>();
            assertState(playlist, null, EmptyListIndex);
        }

        #endregion
    }
}
