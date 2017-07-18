using System;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.YouTube
{
    public struct YouTubeID : IEquatable<YouTubeID>
    {
        private string _id;
        public static YouTubeID Empty { get; } = new YouTubeID(null);

        static YouTubeID()
        {
            Empty = new YouTubeID(null);
        }

        private YouTubeID(string id)
        {
            _id = id;
        }

        public static YouTubeID Parse(string s)
        {
            if (TryParse(s, out YouTubeID id))
                return id;
            else
                throw new ArgumentException("String could not be parsed to a YouTube identifier.", "s");
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
            get { return _id; }
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
            return this._id.Equals(other._id);
        }
        public override int GetHashCode()
        {
            return (_id == null) ? 0 : _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id;
        }
    }
}
