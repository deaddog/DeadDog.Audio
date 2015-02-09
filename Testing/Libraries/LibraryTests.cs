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
        private readonly RawTrack rawTrack1 = new RawTrack("C:\\1.mp3", "Enter Sandman", "Metallica", 1, "Metallica", 1991);

        private RawTrack getRawTrackWithTrackTitle(RawTrack origin, string title)
        {
            return new RawTrack(origin.FullFilename, title, origin.AlbumTitle, origin.TrackNumber, origin.ArtistName, origin.Year);
        }

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

            if (expected.AlbumTitle == null)
                Assert.IsTrue(actual.Album.IsUnknown);
            else
                Assert.AreEqual(expected.AlbumTitle, actual.Album.Title);

            if (expected.ArtistName == null)
                Assert.IsTrue(actual.Artist.IsUnknown);
            else
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

        /// <summary>
        /// Adds a track to the library. 
        /// This method should be used by all methods for setting up the testing library.
        /// This method should never be used for testing the Add method.
        /// </summary>
        private Track addTrack(RawTrack rawTrack)
        {
            return library.AddTrack(rawTrack);
        }

        [TestMethod()]
        public void LibraryTest()
        {
            Library library = new Library();

            assertCounts(0, 0, 0);
        }

        #region Adding

        [TestMethod()]
        public void AddTrackTest()
        {
            var track = library.AddTrack(rawTrack1);

            assertCounts(1, 1, 1);
            assertTrack(rawTrack1, track);
        }

        [TestMethod()]
        public void AddTrackNoTitleTest()
        {
            var rawTrack = getRawTrackWithTrackTitle(rawTrack1, null);
            var track = library.AddTrack(rawTrack);

            assertCounts(1, 1, 1);
            assertTrack(rawTrack, track);
        }

        #endregion

        #region Updating

        [TestMethod()]
        public void UpdateNothingTest()
        {
            var track = addTrack(rawTrack1);
            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            assertCounts(1, 1, 1);
            assertTrack(rawTrack1, track);
        }

        [TestMethod()]
        public void UpdateTitleTest()
        {
            RawTrack rawTrack = getRawTrackWithTrackTitle(rawTrack1, "Wrong Title");
            var track = addTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            assertCounts(1, 1, 1);
            assertTrack(rawTrack1, track);
        }

        [TestMethod()]
        public void UpdateTitleAddTest()
        {
            RawTrack rawTrack = getRawTrackWithTrackTitle(rawTrack1, null);
            var track = addTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            assertCounts(1, 1, 1);
            assertTrack(rawTrack1, track);
        }

        [TestMethod()]
        public void UpdateTitleRemoveTest()
        {
            var track = addTrack(rawTrack1);

            RawTrack rawTrack = getRawTrackWithTrackTitle(rawTrack1, null);
            Assert.AreSame(track, library.UpdateTrack(rawTrack));

            assertCounts(1, 1, 1);
            assertTrack(rawTrack, track);
        }

        #endregion
    }
}
