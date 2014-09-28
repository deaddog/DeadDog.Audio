using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.YouTube
{
    public struct YouTubeID : IEquatable<YouTubeID>
    {
        private string id;
        public static readonly YouTubeID Empty;

        static YouTubeID()
        {
            Empty = new YouTubeID(null);
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
                string idStr = match.Groups["id"].Value;
                return new YouTubeID(idStr);
            }
        }
        public static bool TryParse(string s, out YouTubeID id)
        {
            var match = Regex.Match(s, @"([\?\&]v=(?<id>[^&]+))|^(?<id>([a-zA-Z0-9_\-])+)$");
            if (!match.Success)
            {
                id = Empty;
                return false;
            }
            else
            {
                string idStr = match.Groups["id"].Value;
                id = new YouTubeID(idStr);
                return true;
            }
        }

        public string ID
        {
            get { return id; }
        }

        public static bool operator ==(YouTubeID a, YouTubeID b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(YouTubeID a, YouTubeID b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is YouTubeID)
                return Equals((YouTubeID)obj);
            else
                return false;
        }
        public bool Equals(YouTubeID other)
        {
            return this.id.Equals(other.id);
        }

        public override int GetHashCode()
        {
            return (id == null) ? 0 : id.GetHashCode();
        }

        public override string ToString()
        {
            return id;
        }
    }
}
