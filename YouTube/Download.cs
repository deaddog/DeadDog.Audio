using System;

namespace DeadDog.Audio.YouTube
{
    public class Download
    {
        private YouTubeID id;
        private string path;
        private string title;
        private States state;

        public Download(YouTubeID id, string path)
        {
            this.id = id;
            this.path = path;
            this.title = null;
            this.state = States.None;
        }

        public YouTubeID ID
        {
            get { return id; }
        }
        public string Path
        {
            get { return path; }
        }
        public string Title
        {
            get { return title; }
        }
        public States State
        {
            get { return state; }
        }

        internal void Start()
        {
            throw new NotImplementedException();
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

        public enum States
        {
            None,
            Loading,
            Loaded
        }
    }
}
