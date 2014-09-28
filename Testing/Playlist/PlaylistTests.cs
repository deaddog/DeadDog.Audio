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
    }
}
