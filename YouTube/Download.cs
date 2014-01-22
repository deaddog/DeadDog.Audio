namespace DeadDog.Audio.YouTube
{
    public class Download
    {
        private YouTubeID id;
        private string path;
        private string title;

        public Download(YouTubeID id, string path)
        {
            this.id = id;
            this.path = path;
            this.title = null;
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
            internal set { title = value; }
        }
    }
}
