using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadDog.Audio.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DeadDog.Audio.Libraries.Tests
{
    [TestClass()]
    public class LibraryTests
    {
        private Library library;

        [TestInitialize]
        public void Initialize()
        {
            library = new Library();
        }

        private void assertTrack(RawTrack expected, Track actual)
        {
            Assert.AreEqual(expected.FullFilename, actual.FilePath);
            Assert.AreEqual(expected.TrackTitle, actual.Title);
            Assert.AreEqual(expected.AlbumTitle, actual.Album.Title);
            Assert.AreEqual(expected.ArtistName, actual.Artist.Name);
            Assert.AreEqual(expected.TrackNumberUnknown, !actual.Tracknumber.HasValue);
            if (!expected.TrackNumberUnknown)
                Assert.AreEqual(expected.TrackNumber, actual.Tracknumber.Value);
        }
        private void assertCounts(int artistCount, int albumCount, int trackCount)
        {
            Assert.AreEqual(artistCount, library.Artists.Count);
            Assert.AreEqual(albumCount, library.Albums.Count);
            Assert.AreEqual(trackCount, library.Tracks.Count);
        }

        [TestMethod()]
        public void LibraryTest()
        {
            Library library = new Library();

            assertCounts(0, 0, 0);
        }

        #region Adding

        [TestMethod()]
        public void AddTrackTest1()
        {
            RawTrack rawTrack = new RawTrack("C:\\1.mp3", "Enter Sandman", "Metallica", 1, "Metallica", 1991);
            var track = library.AddTrack(rawTrack);

            assertCounts(1, 1, 1);
            assertTrack(rawTrack, track);
        }

        [TestMethod()]
        public void AddTrackTest2()
        {
            RawTrack rawTrack = new RawTrack("C:\\1.mp3", null, "Metallica", 1, "Metallica", 1991);
            var track = library.AddTrack(rawTrack);

            assertCounts(1, 1, 1);
            assertTrack(rawTrack, track);
        }

        #endregion

        #region Updating

        [TestMethod()]
        public void UpdateNothingTest()
        {
            RawTrack rawTrack = new RawTrack("C:\\1.mp3", "Enter Sandman", "Metallica", 1, "Metallica", 1991);
            var track = library.AddTrack(rawTrack);

            rawTrack = new RawTrack("C:\\1.mp3", "Enter Sandman", "Metallica", 1, "Metallica", 1991);
            Assert.AreSame(track, library.UpdateTrack(rawTrack));

            assertCounts(1, 1, 1);
            assertTrack(rawTrack, track);
        }

        #endregion
    }
}
