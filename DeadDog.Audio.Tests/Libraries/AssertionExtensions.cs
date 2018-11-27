using DeadDog.Audio.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeadDog.Audio.Tests.Libraries
{
    public static class AssertionExtensions
    {
        public static void AssertSizes(this Library library, int artistCount, int albumCount, int trackCount)
        {
            Assert.AreEqual(artistCount, library.Artists.Count, "Artist count mismatch.");
            Assert.AreEqual(albumCount, library.Albums.Count, "Album count mismatch.");
            Assert.AreEqual(trackCount, library.Tracks.Count, "Track count mismatch.");
        }

        public static void AssertTrack(this Library library, RawTrack track)
        {
            if(!library.TryGet(track, out var actual))
                Assert.Fail("Track path \"" + track.Filepath + "\" not found in library.");

            AssertTrack
            (
                library: library,
                expected: track,
                actual: actual
            );
        }
        public static void AssertTrack(this Library library, RawTrack expected, Track actual)
        {
            Assert.IsTrue(library.Contains(expected), "Track path \"" + expected.Filepath + "\" not found in library.");
            Assert.IsTrue(library.Tracks.Contains(actual), "Track instance not found in library.");

            Assert.IsTrue(library.TryGet(expected, out var track) && ReferenceEquals(track, actual));

            Assert.AreEqual(expected.Filepath, actual.FilePath, "Different file paths, probable error in test.");
            Assert.AreEqual(expected.TrackTitle, actual.Title, "Title mismatch.");

            if (actual.Album == null)
                Assert.Fail("Track does not have an album.");
            else if (expected.AlbumTitle == null)
                Assert.IsTrue(actual.Album.IsUnknown, "Expected unknown album.");
            else
            {
                Assert.IsTrue(library.Albums.Contains(actual.Album), "Album instance not found in library.");
                Assert.AreEqual(expected.AlbumTitle, actual.Album.Title, "Album title mismatch.");
            }

            if (actual.Artist == null)
                Assert.Fail("Track does not have an artist.");
            else if (expected.ArtistName == null)
                Assert.IsTrue(actual.Artist.IsUnknown, "Expected unknown artist.");
            else
            {
                Assert.IsTrue(library.Artists.Contains(actual.Artist), "Artist instance not found in library.");
                Assert.AreEqual(expected.ArtistName, actual.Artist.Name, "Artist name mismatch.");
            }

            Assert.AreEqual(expected.TrackNumber, actual.Tracknumber, "Track number mismatch.");
        }
    }
}
