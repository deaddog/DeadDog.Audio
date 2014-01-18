using System;
using System.Collections.Generic;

namespace DeadDog.Audio.YouTube
{
    public class Downloader
    {
        private string directory;

        public Downloader(string directory)
        {
            this.directory = System.IO.Path.GetFullPath(directory).TrimEnd('\\');
        }

        private string getClipPath(YouTubeID id)
        {
            return directory + "\\" + id.ID + ".mp3";
        }
    }
}
