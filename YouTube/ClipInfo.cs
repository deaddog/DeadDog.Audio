using System;

namespace DeadDog.Audio.YouTube
{
    public class ClipInfo
    {
        private string youtubeID;

        public ClipInfo(string youtubeID)
        {
            this.youtubeID = youtubeID;
        }

        public string YouTubeID
        {
            get { return youtubeID; }
        }
    }
}
