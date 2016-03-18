using System;

namespace DeadDog.Audio.Playback
{
    public class FilePlayback<T> : IPlayback<T>
    {
        private readonly IFilePlayback playback;

        public FilePlayback(IFilePlayback playback)
        {
            if (playback == null)
                throw new ArgumentNullException(nameof(playback));

            this.playback = playback;
        }
    }
}
