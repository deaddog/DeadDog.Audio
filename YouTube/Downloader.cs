using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public class Downloader
    {
        private string directory;
        private string documentPath;
        private XDocument document;
        private Dictionary<YouTubeID, State> files;

        public Downloader(string directory)
        {
            this.directory = Path.GetFullPath(directory);
            this.files = new Dictionary<YouTubeID, State>();

            this.documentPath = XML.DocumentPath(directory);
            System.IO.FileInfo file = new System.IO.FileInfo(documentPath);
            if (!file.Exists)
                document = new XDocument(new XElement("tracks"));
            else
                document = XDocument.Load(directory);
        }

        public void Load(YouTubeID id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            lock (files)
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
            return Path.Combine(directory, +id.ID + ".mp3");
        }

        public enum State
        {
            None,
            Loading,
            Loaded
        }
    }
}
