using System;
using System.Collections.Generic;
using System.Threading;

namespace DeadDog.Audio.YouTube
{
    public class Downloader
    {
        private string directory;
        private Dictionary<YouTubeID, State> files;

        private readonly object dictionaryLock = new object();

        public Downloader(string directory)
        {
            this.directory = System.IO.Path.GetFullPath(directory).TrimEnd('\\');
            this.files = new Dictionary<YouTubeID, State>();
        }

        public void Load(YouTubeID id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            lock (dictionaryLock)
            {
                if (files.ContainsKey(id))
                    return;
                else
                {
                    files.Add(id, State.Loading);
                    Thread thread = new Thread(() => performLoad(id));
                    thread.Start();
                }
            }
        }

        private void performLoad(YouTubeID id)
        {
            throw new NotImplementedException();
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
