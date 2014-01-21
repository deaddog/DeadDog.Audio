using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.YouTube
{
    public class YouTubeID
    {
        private string id;
        private static Dictionary<string, YouTubeID> lookup;

        static YouTubeID()
        {
            lookup = new Dictionary<string, YouTubeID>();
        }

        private YouTubeID(string id)
        {
            this.id = id;
        }

        public static YouTubeID Parse(string s)
        {
            var match = Regex.Match(s, @"([\?\&]v=(?<id>[^&]+))|^(?<id>([a-zA-Z0-9_\-])+)$");
            if (!match.Success)
                throw new ArgumentException("String could not be parsed to a YouTube identifier.", "s");
            else
            {
                YouTubeID id;
                string idStr = match.Groups["id"].Value;
                lock (lookup)
                    if (!lookup.TryGetValue(idStr, out id))
                    {
                        id = new YouTubeID(idStr);
                        lookup.Add(idStr, id);
                    }
                return id;
            }
        }
        public static bool TryParse(string s, out YouTubeID id)
        {
            var match = Regex.Match(s, @"([\?\&]v=(?<id>[^&]+))|^(?<id>([a-zA-Z0-9_\-])+)$");
            if (!match.Success)
            {
                id = null;
                return false;
            }
            else
            {
                string idStr = match.Groups["id"].Value;
                lock (lookup)
                    if (!lookup.TryGetValue(idStr, out id))
                    {
                        id = new YouTubeID(idStr);
                        lookup.Add(idStr, id);
                    }
                return true;
            }
        }

        public string ID
        {
            get { return id; }
        }
    }
}
