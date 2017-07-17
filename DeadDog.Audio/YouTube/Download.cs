using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public class Download
    {
        private const int MAX_ATTEMPTS = 3;
        private const int THREAD_SLEEP = 500;
        private const int BUFFER_SIZE = 8192;

        private YouTubeID id;
        private string path;
        private string title;
        private RawTrack trackinfo;
        private States state;
        private long size;
        private long downloaded;

        internal Download(YouTubeID id, string path)
        {
            if (id == YouTubeID.Empty)
                throw new ArgumentException("YouTubeID.Empty is not a valid argument.", "id");
            if (path == null)
                throw new ArgumentNullException("path");

            this.id = id;
            this.path = path;
            this.title = null;
            this.state = States.None;
            this.size = -1;
            this.downloaded = -1;
        }
        internal Download(YouTubeID id, RawTrack trackinfo, string title)
            : this(id, trackinfo.Filepath)
        {
            if (trackinfo == null)
                throw new ArgumentNullException("trackinfo");
            if (title == null)
                throw new ArgumentNullException("title");

            this.trackinfo = trackinfo;
            this.title = title;
            this.state = States.Loaded;
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
        public RawTrack TrackInfo
        {
            get { return trackinfo; }
            internal set { trackinfo = value; }
        }
        public States State
        {
            get { return state; }
        }
        public long Size
        {
            get { return size; }
        }
        public long Downloaded
        {
            get { return downloaded; }
        }

        internal void Start(Action<Download> onComplete)
        {
            this.state = States.LoadingTitle;

            try
            {
                URL infoURL = new URL("https://gdata.youtube.com/feeds/api/videos/" + this.id.ID + "?v=2");

                string xml = infoURL.GetHTML(System.Text.Encoding.UTF8).Trim('\0');
                XDocument doc = XDocument.Load(new System.IO.StringReader(xml));

                this.title = doc == null ? string.Empty : doc.Root.Element(XName.Get("title", "http://www.w3.org/2005/Atom")).Value;

                this.state = States.LoadingFile;

                URL mp3URL = Sources.YouTubeInMp3(this.id);
                using (FileStream fs = new FileStream(this.path, FileMode.Create, FileAccess.Write))
                    loadToStream(mp3URL, fs);

                state = States.Loaded;
            }
            catch { state = States.Failed; }

            onComplete(this);
            if (state == States.Loaded)
                state = States.Completed;
        }

        private void loadToStream(URL url, Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("The stream must support writing.", "stream");

            byte[] buf = new byte[BUFFER_SIZE];

            HttpWebRequest request;
            HttpWebResponse response = null;
            Stream resStream = null;
            int attempt = 0;

            while (attempt < MAX_ATTEMPTS && resStream == null)
                try
                {
                    attempt++;

                    request = (HttpWebRequest)WebRequest.Create(url.Address);
                    response = (HttpWebResponse)request.GetResponse();

                    resStream = response.GetResponseStream();
                }
                catch
                {
                    System.Threading.Thread.Sleep(THREAD_SLEEP);
                    if (attempt == MAX_ATTEMPTS)
                        throw new Exception("File could not be loaded after " + MAX_ATTEMPTS + " attempts.");
                }

            if (response.ContentLength > int.MaxValue)
                throw new InvalidOperationException("Cannot read files larger than 2gb (UINT32 max)");
            size = response.ContentLength;
            downloaded = 0;

            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                downloaded += count;
                if (count > 0)
                    stream.Write(buf, 0, count);
            }
            while (count > 0);
            downloaded = size;
            resStream.Dispose();
        }

        public enum States
        {
            None,
            LoadingTitle,
            LoadingFile,
            Loaded,
            Failed,
            Completed
        }
    }
}
