using System;
using System.Collections.Generic;

namespace DeadDog.Audio.YouTube
{
    public class Downloader
    {
        private string directory;
        private Dictionary<YouTubeID, State> files;

        public Downloader(string directory)
        {
            this.directory = System.IO.Path.GetFullPath(directory).TrimEnd('\\');
            this.files = new Dictionary<YouTubeID, State>();
        }

        public void Load(YouTubeID id)
        {
        }

        private string getClipPath(YouTubeID id)
        {
            return directory + "\\" + id.ID + ".mp3";
        }

        public enum State
        {
            None,
            Loading,
            Loaded
        }
    }
}
