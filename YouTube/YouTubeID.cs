using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.YouTube
{
    public class YouTubeID
    {
        private string id;

        private YouTubeID(string id)
        {
            this.id = id;
        }

        public string ID
        {
            get { return id; }
        }
    }
}
