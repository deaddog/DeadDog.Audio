using DeadDog.Audio.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DeadDog.Audio.Tests.Libraries
{
    [TestClass]
    public class LibraryComparisonsTests
    {
        [TestMethod]
        [Description("Tests that tracks on the same album are sorted together regardless of their artist.")]
        public void TwoAlbumsThreeArtistsTest()
        {
            var library = new Library();

            var NoAlbum1 = library.Add(new RawTrack("C:\\NoAlbum1.mp3", "My Song", "Great Album", 1, "One Artist", null));
            var NoAlbum2 = library.Add(new RawTrack("C:\\NoAlbum2.mp3", "My Other Song", "Great Album", 2, "An Artist", null));

            var Track1 = library.Add(new RawTrack("C:\\1.mp3", "A song", "Album", 1, "Artist", null));
            var Track2 = library.Add(new RawTrack("C:\\2.mp3", "Another song", "Album", 2, "Artist", null));

            var allTracks = library.Tracks.ToList();

            Assert.AreEqual(Track1, allTracks[0]);
            Assert.AreEqual(Track2, allTracks[1]);
            Assert.AreEqual(NoAlbum1, allTracks[2]);
            Assert.AreEqual(NoAlbum2, allTracks[3]);
        }
    }
}
