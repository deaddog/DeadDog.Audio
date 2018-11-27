using DeadDog.Audio.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Libraries
{
    [TestClass]
    public class LibraryTests
    {
        private RawTrack Track1 { get; } = new RawTrack("C:\\1.mp3", "Ain't My Bitch", "Load", 1, "Metallica", 1991);
        private RawTrack Track2 { get; } = new RawTrack("C:\\2.mp3", "2 X 4", "Load", 2, "Metallica", 1991);
        private RawTrack Track3 { get; } = new RawTrack("C:\\3.mp3", "The House Jack Built", "Load", 3, "Metallica", 1991);

        private Library _library;

        [TestInitialize]
        public void Initialize()
        {
            _library = new Library();
        }

        [TestMethod]
        public void LibraryTest()
        {
            _library.AssertSizes(0, 0, 0);
        }

        #region Adding

        [TestMethod]
        public void AddTrackTest()
        {
            var track = _library.Add(Track1);

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }

        [TestMethod]
        public void AddTrackNoTitleTest()
        {
            var rawTrack = Track1.WithoutTrackTitle();
            var track = _library.Add(rawTrack);

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void AddTrackNoAlbumTest()
        {
            var rawTrack = Track1.WithoutAlbumTitle();
            var track = _library.Add(rawTrack);

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void AddTrackNoArtistTest()
        {
            var rawTrack = Track1.WithoutArtistName();
            var track = _library.Add(rawTrack);

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        #endregion

        #region Updating

        [TestMethod]
        public void UpdateNothingTest()
        {
            var track = _library.Add(Track1);
            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }

        [TestMethod]
        public void UpdateTitleTest()
        {
            RawTrack rawTrack = Track1.WithTrackTitle("Wrong Title");
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateTitleAddTest()
        {
            RawTrack rawTrack = Track1.WithoutTrackTitle();
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateTitleRemoveTest()
        {
            var track = _library.Add(Track1);

            RawTrack rawTrack = Track1.WithoutTrackTitle();
            Assert.AreSame(track, _library.Update(rawTrack));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void UpdateAlbumTest()
        {
            RawTrack rawTrack = Track1.WithAlbumTitle("Wrong Title");
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateAlbumAddTest()
        {
            RawTrack rawTrack = Track1.WithoutAlbumTitle();
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateAlbumRemoveTest()
        {
            var track = _library.Add(Track1);

            RawTrack rawTrack = Track1.WithoutAlbumTitle();
            Assert.AreSame(track, _library.Update(rawTrack));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void UpdateArtistTest()
        {
            RawTrack rawTrack = Track1.WithArtistName("Wrong Name");
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateArtistAddTest()
        {
            RawTrack rawTrack = Track1.WithoutArtistName();
            var track = _library.Add(rawTrack);

            Assert.AreSame(track, _library.Update(Track1));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, track);
        }
        [TestMethod]
        public void UpdateArtistRemoveTest()
        {
            var track = _library.Add(Track1);

            RawTrack rawTrack = Track1.WithoutArtistName();
            Assert.AreSame(track, _library.Update(rawTrack));

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(rawTrack, track);
        }

        #endregion

        [TestMethod]
        [Description("Tests that removing a track does not double-add an album to an artist.")]
        public void DoubleAlbumErrorTest()
        {
            var t1 = _library.Add(Track1);
            var t2 = _library.Add(Track2);

            _library.Remove(t2);

            _library.AssertSizes(1, 1, 1);
            _library.AssertTrack(Track1, t1);
            Assert.AreEqual(1, t1.Artist.Albums.Count);
        }

        [TestMethod]
        [Description("Tests that removing a track from an album such that all tracks have same artist will set the artist of the album.")]
        public void ResetAlbumArtistErrorTest()
        {
            var t1 = _library.Add(Track1);
            var t2 = _library.Add(Track2);
            var t3 = _library.Add(Track3.WithArtistName("Different Name"));

            Assert.IsTrue(t1.Album.Artist == null, "Various artists album should not have an artist.");

            _library.Remove(t3);

            Assert.IsFalse(t1.Album.Artist.IsUnknown, "Album should no longer have various artists.");
            Assert.AreSame(t1.Artist, t1.Album.Artist, "Album artist does not match track artist.");
            Assert.IsTrue(t1.Artist.Albums.Contains(t1.Album), "Artist does not contain album.");
        }

        [TestMethod]
        [Description("Tests that changing a track-number will not affect sorting (1,2) -> (1,3).")]
        public void NoMoveTest()
        {
            var t1 = _library.Add(Track1);
            var t2 = _library.Add(Track2);

            _library.Update(Track2.WithTrackNumber(3));

            _library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t1, _library.Tracks[0]);
            Assert.AreEqual(t2, _library.Tracks[1]);

            Assert.AreEqual(t1, _library.Albums[0].Tracks[0]);
            Assert.AreEqual(t2, _library.Albums[0].Tracks[1]);

            Assert.AreEqual(t1, _library.Artists[0].Tracks[0]);
            Assert.AreEqual(t2, _library.Artists[0].Tracks[1]);
        }

        [TestMethod]
        [Description("Tests that lowering a track-number will affect sorting.")]
        public void MoveLeftTest()
        {
            var t2 = _library.Add(Track2);
            var t3 = _library.Add(Track3);

            _library.Update(Track3.WithTrackNumber(1));

            _library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t3, _library.Tracks[0]);
            Assert.AreEqual(t2, _library.Tracks[1]);

            Assert.AreEqual(t3, _library.Albums[0].Tracks[0]);
            Assert.AreEqual(t2, _library.Albums[0].Tracks[1]);

            Assert.AreEqual(t3, _library.Artists[0].Tracks[0]);
            Assert.AreEqual(t2, _library.Artists[0].Tracks[1]);
        }

        [TestMethod]
        [Description("Tests that raising a track-number will affect sorting.")]
        public void MoveRightTest()
        {
            var t1 = _library.Add(Track1);
            var t2 = _library.Add(Track2);

            _library.Update(Track1.WithTrackNumber(3));

            _library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t2, _library.Tracks[0]);
            Assert.AreEqual(t1, _library.Tracks[1]);

            Assert.AreEqual(t2, _library.Albums[0].Tracks[0]);
            Assert.AreEqual(t1, _library.Albums[0].Tracks[1]);

            Assert.AreEqual(t2, _library.Artists[0].Tracks[0]);
            Assert.AreEqual(t1, _library.Artists[0].Tracks[1]);
        }
    }
}
