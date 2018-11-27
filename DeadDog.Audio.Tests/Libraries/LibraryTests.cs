using DeadDog.Audio.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Libraries
{
    [TestClass]
    public class LibraryTests
    {
        private readonly RawTrack rawTrack1 = new RawTrack("C:\\1.mp3", "Ain't My Bitch", "Load", 1, "Metallica", 1991);
        private readonly RawTrack rawTrack2 = new RawTrack("C:\\2.mp3", "2 X 4", "Load", 2, "Metallica", 1991);
        private readonly RawTrack rawTrack3 = new RawTrack("C:\\3.mp3", "The House Jack Built", "Load", 3, "Metallica", 1991);

        private Library library;

        [TestInitialize]
        public void Initialize()
        {
            library = new Library();
        }

        [TestMethod]
        public void LibraryTest()
        {
            Library library = new Library();

            library.AssertSizes(0, 0, 0);
        }

        #region Adding

        [TestMethod]
        public void AddTrackTest()
        {
            var track = library.AddTrack(rawTrack1);

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }

        [TestMethod]
        public void AddTrackNoTitleTest()
        {
            var rawTrack = rawTrack1.WithoutTrackTitle();
            var track = library.AddTrack(rawTrack);

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void AddTrackNoAlbumTest()
        {
            var rawTrack = rawTrack1.WithoutAlbumTitle();
            var track = library.AddTrack(rawTrack);

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void AddTrackNoArtistTest()
        {
            var rawTrack = rawTrack1.WithoutArtistName();
            var track = library.AddTrack(rawTrack);

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        #endregion

        #region Updating

        [TestMethod]
        public void UpdateNothingTest()
        {
            var track = library.AddTrack(rawTrack1);
            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }

        [TestMethod]
        public void UpdateTitleTest()
        {
            RawTrack rawTrack = rawTrack1.WithTrackTitle("Wrong Title");
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateTitleAddTest()
        {
            RawTrack rawTrack = rawTrack1.WithoutTrackTitle();
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateTitleRemoveTest()
        {
            var track = library.AddTrack(rawTrack1);

            RawTrack rawTrack = rawTrack1.WithoutTrackTitle();
            Assert.AreSame(track, library.UpdateTrack(rawTrack));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void UpdateAlbumTest()
        {
            RawTrack rawTrack = rawTrack1.WithAlbumTitle("Wrong Title");
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateAlbumAddTest()
        {
            RawTrack rawTrack = rawTrack1.WithoutAlbumTitle();
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateAlbumRemoveTest()
        {
            var track = library.AddTrack(rawTrack1);

            RawTrack rawTrack = rawTrack1.WithoutAlbumTitle();
            Assert.AreSame(track, library.UpdateTrack(rawTrack));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        [TestMethod]
        public void UpdateArtistTest()
        {
            RawTrack rawTrack = rawTrack1.WithArtistName("Wrong Name");
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateArtistAddTest()
        {
            RawTrack rawTrack = rawTrack1.WithoutArtistName();
            var track = library.AddTrack(rawTrack);

            Assert.AreSame(track, library.UpdateTrack(rawTrack1));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, track);
        }
        [TestMethod]
        public void UpdateArtistRemoveTest()
        {
            var track = library.AddTrack(rawTrack1);

            RawTrack rawTrack = rawTrack1.WithoutArtistName();
            Assert.AreSame(track, library.UpdateTrack(rawTrack));

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack, track);
        }

        #endregion

        [TestMethod]
        [Description("Tests that removing a track does not double-add an album to an artist.")]
        public void DoubleAlbumErrorTest()
        {
            var t1 = library.AddTrack(rawTrack1);
            var t2 = library.AddTrack(rawTrack2);

            library.RemoveTrack(t2);

            library.AssertSizes(1, 1, 1);
            library.AssertTrack(rawTrack1, t1);
            Assert.AreEqual(1, t1.Artist.Albums.Count);
        }

        [TestMethod]
        [Description("Tests that removing a track from an album such that all tracks have same artist will set the artist of the album.")]
        public void ResetAlbumArtistErrorTest()
        {
            var t1 = library.AddTrack(rawTrack1);
            var t2 = library.AddTrack(rawTrack2);
            var t3 = library.AddTrack(rawTrack3.WithArtistName("Different Name"));

            Assert.IsTrue(t1.Album.Artist == null, "Various artists album should not have an artist.");

            library.RemoveTrack(t3);

            Assert.IsFalse(t1.Album.Artist.IsUnknown, "Album should no longer have various artists.");
            Assert.AreSame(t1.Artist, t1.Album.Artist, "Album artist does not match track artist.");
            Assert.IsTrue(t1.Artist.Albums.Contains(t1.Album), "Artist does not contain album.");
        }

        [TestMethod]
        [Description("Tests that changing a track-number will not affect sorting (1,2) -> (1,3).")]
        public void NoMoveTest()
        {
            var t1 = library.AddTrack(rawTrack1);
            var t2 = library.AddTrack(rawTrack2);

            library.UpdateTrack(rawTrack2.WithTrackNumber(3));

            library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t1, library.Tracks[0]);
            Assert.AreEqual(t2, library.Tracks[1]);

            Assert.AreEqual(t1, library.Albums[0].Tracks[0]);
            Assert.AreEqual(t2, library.Albums[0].Tracks[1]);

            Assert.AreEqual(t1, library.Artists[0].Tracks[0]);
            Assert.AreEqual(t2, library.Artists[0].Tracks[1]);
        }

        [TestMethod]
        [Description("Tests that lowering a track-number will affect sorting.")]
        public void MoveLeftTest()
        {
            var t2 = library.AddTrack(rawTrack2);
            var t3 = library.AddTrack(rawTrack3);

            library.UpdateTrack(rawTrack3.WithTrackNumber(1));

            library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t3, library.Tracks[0]);
            Assert.AreEqual(t2, library.Tracks[1]);

            Assert.AreEqual(t3, library.Albums[0].Tracks[0]);
            Assert.AreEqual(t2, library.Albums[0].Tracks[1]);

            Assert.AreEqual(t3, library.Artists[0].Tracks[0]);
            Assert.AreEqual(t2, library.Artists[0].Tracks[1]);
        }

        [TestMethod]
        [Description("Tests that raising a track-number will affect sorting.")]
        public void MoveRightTest()
        {
            var t1 = library.AddTrack(rawTrack1);
            var t2 = library.AddTrack(rawTrack2);

            library.UpdateTrack(rawTrack1.WithTrackNumber(3));

            library.AssertSizes(1, 1, 2);

            Assert.AreEqual(t2, library.Tracks[0]);
            Assert.AreEqual(t1, library.Tracks[1]);

            Assert.AreEqual(t2, library.Albums[0].Tracks[0]);
            Assert.AreEqual(t1, library.Albums[0].Tracks[1]);

            Assert.AreEqual(t2, library.Artists[0].Tracks[0]);
            Assert.AreEqual(t1, library.Artists[0].Tracks[1]);
        }
    }
}
