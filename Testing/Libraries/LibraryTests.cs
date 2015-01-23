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

        [TestMethod()]
        public void LibraryTest()
        {
            Library library = new Library();

            Assert.AreEqual(0, library.Tracks.Count);
            Assert.AreEqual(0, library.Artists.Count);
            Assert.AreEqual(0, library.Albums.Count);
        }

        [TestMethod()]
        public void AddTrackTest1()
        {
            RawTrack rawTrack = new RawTrack("C:\\1.mp3", "Enter Sandman", "Metallica", 1, "Metallica", 1991);
            var track = library.AddTrack(rawTrack);

            Assert.AreEqual(1, library.Tracks.Count);
            Assert.AreEqual(1, library.Artists.Count);
            Assert.AreEqual(1, library.Albums.Count);

            Assert.AreEqual(rawTrack.FullFilename, track.FilePath);
            Assert.AreEqual(rawTrack.TrackTitle, track.Title);
            Assert.AreEqual(rawTrack.AlbumTitle, track.Album.Title);
            Assert.AreEqual(rawTrack.ArtistName, track.Artist.Name);
            Assert.AreEqual(rawTrack.TrackNumberUnknown, false);
            Assert.AreEqual(rawTrack.TrackNumber, track.Tracknumber);
        }

        [TestMethod()]
        public void AddTrackTest2()
        {
            RawTrack rawTrack = new RawTrack("C:\\1.mp3", null, "Metallica", 1, "Metallica", 1991);
            var track = library.AddTrack(rawTrack);

            Assert.AreEqual(1, library.Tracks.Count);
            Assert.AreEqual(1, library.Artists.Count);
            Assert.AreEqual(1, library.Albums.Count);

            Assert.AreEqual(rawTrack.FullFilename, track.FilePath);
            Assert.AreEqual(rawTrack.TrackTitle, track.Title);
            Assert.AreEqual(rawTrack.AlbumTitle, track.Album.Title);
            Assert.AreEqual(rawTrack.ArtistName, track.Artist.Name);
            Assert.AreEqual(rawTrack.TrackNumberUnknown, false);
            Assert.AreEqual(rawTrack.TrackNumber, track.Tracknumber);
        }
    }
}
