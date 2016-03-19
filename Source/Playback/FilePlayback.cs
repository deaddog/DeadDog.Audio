﻿using System;

namespace DeadDog.Audio.Playback
{
    public class FilePlayback<T> : IPlayback<T>
    {
        private readonly IFilePlayback playback;
        private readonly Func<T, string> getFilename;

        private uint trackLength;
        private uint trackPosition;

        public FilePlayback(IFilePlayback playback, Func<T, string> getFilename)
        {
            if (playback == null)
                throw new ArgumentNullException(nameof(playback));
            if (getFilename == null)
                throw new ArgumentNullException(nameof(getFilename));

            this.playback = playback;
            this.getFilename = getFilename;

            this.trackLength = 0;
            this.trackPosition = 0;
        }

        public bool CanOpen(T element)
        {
            if (element == null)
                return false;

            string fullpath;

            try { fullpath = System.IO.Path.GetFullPath(getFilename(element)); }
            catch { fullpath = null; }
            if (!System.IO.File.Exists(fullpath))
                return false;

            return playback.CanOpen(fullpath);
        }

        public uint Length => trackLength;
        public uint Position => trackPosition;
    }
}
