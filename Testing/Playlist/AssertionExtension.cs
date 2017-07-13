using DeadDog.Audio.Playlist;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DeadDog.Audio.Tests.Playlist
{
    public static class AssertionExtension
    {
        public static void AssertState(this IPlaylist<string> playlist, string expectedEntry) => Assert.AreEqual(expectedEntry, playlist.Entry);
        public static void AssertState(this Playlist<string> playlist, string expectedEntry, int expectedIndex)
        {
            AssertState(playlist, expectedEntry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }
        public static void AssertState(this PlaylistCollection<string> playlist, string expectedEntry, int expectedIndex)
        {
            AssertState(playlist, expectedEntry);
            Assert.AreEqual(expectedIndex, playlist.Index);
        }

        public static void AssertEmpty(this Playlist<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.EmptyListIndex);
        }
        public static void AssertEmpty(this PlaylistCollection<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.EmptyListIndex);
        }

        public static void AssertStatePre(this Playlist<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.PreListIndex);
        }
        public static void AssertStatePre(this PlaylistCollection<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.PreListIndex);
        }

        public static void AssertStatePost(this Playlist<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.PostListIndex);
        }
        public static void AssertStatePost(this PlaylistCollection<string> playlist)
        {
            AssertState(playlist, null, Playlist<string>.PostListIndex);
        }

        public static void AssertMove(this Playlist<string> playlist, Func<bool> move, bool expectsMove, bool expectsEvent)
        {
            int entryChangeCount = 0;
            void EntryCounter(object sender, EventArgs e) => entryChangeCount++;

            playlist.EntryChanged += EntryCounter;

            var oldIndex = playlist.Index;
            var oldEntry = playlist.Entry;

            Assert.AreEqual(expectsMove, move());
            if (expectsEvent)
                Assert.AreEqual(1, entryChangeCount, "The EntryChanged event was not raised exactly once.");
            else
            {
                AssertState(playlist, oldEntry, oldIndex);
                Assert.AreEqual(0, entryChangeCount, "The EntryChanged event was raised.");
            }

            playlist.EntryChanged -= EntryCounter;
        }
        public static void AssertMove(this Playlist<string> playlist, string target, bool expectedMove, bool expectsEvent)
        {
            AssertMove(playlist, () => playlist.MoveToEntry(target), expectedMove, expectsEvent);
        }
        public static void AssertMove(this PlaylistCollection<string> playlist, Func<bool> move, bool expectsMove, bool expectsEvent)
        {
            int entryChangeCount = 0;
            void EntryCounter(object sender, EventArgs e) => entryChangeCount++;

            playlist.EntryChanged += EntryCounter;

            var oldIndex = playlist.Index;
            var oldEntry = playlist.Entry;

            Assert.AreEqual(expectsMove, move());
            if (expectsEvent)
                Assert.AreEqual(1, entryChangeCount, "The EntryChanged event was not raised exactly once.");
            else
            {
                AssertState(playlist, oldEntry, oldIndex);
                Assert.AreEqual(0, entryChangeCount, "The EntryChanged event was raised.");
            }

            playlist.EntryChanged -= EntryCounter;
        }
        public static void AssertMove(this PlaylistCollection<string> playlist, string target, bool expectedMove, bool expectsEvent)
        {
            AssertMove(playlist, () => playlist.MoveToEntry(target), expectedMove, expectsEvent);
        }

        public static void AssertAllForwards(this Playlist<string> playlist, params string[] entries)
        {
            var index = playlist.Index;

            for (int i = 0; i < entries.Length; i++)
            {
                AssertMove(playlist, playlist.MoveNext, true, true);
                AssertState(playlist, entries[i], index + i + 1);
            }

            AssertMove(playlist, playlist.MoveNext, false, true);
            AssertStatePost(playlist);
        }
        public static void AssertAllBackwards(this Playlist<string> playlist, params string[] entries)
        {
            var index = playlist.Index;

            for (int i = 0; i < entries.Length; i++)
            {
                AssertMove(playlist, playlist.MovePrevious, true, true);
                AssertState(playlist, entries[i], index - i - 1);
            }

            AssertMove(playlist, playlist.MovePrevious, false, true);
            AssertStatePre(playlist);
        }

        public static void AssertMoveEntry(this Playlist<string> playlist, string entry, int newIndex)
        {
            int entryChangeCount = 0;
            void EntryCounter(object sender, EventArgs e) => entryChangeCount++;

            playlist.EntryChanged += EntryCounter;
            
            playlist.MoveEntry(entry, newIndex);
            Assert.AreEqual(0, entryChangeCount, "The EntryChanged event was raised.");

            playlist.EntryChanged -= EntryCounter;
        }
    }
}
