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
        private Dictionary<YouTubeID, Download> files;

        public Downloader(string directory)
        {
            this.directory = Path.GetFullPath(directory);
            this.files = new Dictionary<YouTubeID, State>();

            this.documentPath = XML.DocumentPath(directory);
            System.IO.FileInfo file = new System.IO.FileInfo(documentPath);
            if (!file.Exists)
                document = new XDocument(new XElement("tracks"));
            else
            {
                document = XDocument.Load(documentPath);
                foreach (var e in document.Element("tracks").Elements("track"))
                    files.Add(YouTubeID.Parse(e.Attribute("id").Value), State.Loaded);
            }
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
    }
}
