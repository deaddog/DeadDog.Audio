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

        private void performLoad(YouTubeID id)
        {
            string text;

            URL mp3URL = getMp3URL(id, out text);
            URL infoURL = new URL("https://gdata.youtube.com/feeds/api/videos/" + id.ID + "?v=2");

            string xml = infoURL.GetHTML(System.Text.Encoding.UTF8).Trim('\0');
            XDocument doc = XDocument.Load(new System.IO.StringReader(xml));

            if (doc != null)
                text = doc.Root.Element(XName.Get("title", "http://www.w3.org/2005/Atom")).Value;

            mp3URL.GetFile(Path.Combine(directory, id.ID + ".mp3"));

            XElement track = new XElement("track",
                new XAttribute("id", id.ID),
                new XElement("path", id.ID + ".mp3"),
                new XElement("title", text));

            lock (document)
            {
                document.Element("tracks").Add(track);
                document.Save(documentPath);
            }
            lock (files)
                files[id] = State.Loaded;
        }
        private URL getMp3URL(YouTubeID id, out string text)
        {
            URL mp3infoURL = new URL("http://youtubeinmp3.com/fetch/?api=advanced&video=http://www.youtube.com/watch?v=" + id.ID);
            string html = mp3infoURL.GetHTML();

            int linkIndex = html.IndexOf("<br />Link: ");
            string link = html.Substring(linkIndex + 12);
            link = link.Trim(' ', '\t', '\n', '\r', '\0');

            text = html.Substring(7, html.IndexOf("<br />") - 12).Replace('_', ' ');

            return new URL(link);
        }

        public enum State
        {
            None,
            Loading,
            Loaded
        }
    }
}
