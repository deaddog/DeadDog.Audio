using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public static class YouTubeParser
    {
        private static Dictionary<YouTubeID, string> titles;

        static YouTubeParser()
        {
            titles = new Dictionary<YouTubeID, string>();
        }

        public static RawTrack ParseTrack(YouTubeID id, string filepath)
        {
            Regex dash = new Regex("^(?<artist>[^-]+)-(?<track>[^-]+)$");
            Regex slash = new Regex("^(?<artist>[^/]+)/(?<track>.+)$");
            Match match;

            string youtubeTitle = GetTitle(id) ?? string.Empty;

            string artist = null;
            string album = null;
            string title = youtubeTitle;

            if ((match = dash.Match(youtubeTitle)).Success || (match = slash.Match(youtubeTitle)).Success)
            {
                artist = match.Groups["artist"].Value.Trim();
                title = match.Groups["track"].Value.Trim();
            }

            return new RawTrack(filepath, title, album, RawTrack.TrackNumberIfUnknown, artist, RawTrack.YearIfUnknown);
        }

        public static string GetTitle(YouTubeID id)
        {
            string title;
            if (!titles.TryGetValue(id, out title))
            {
                URL infoURL = new URL("https://gdata.youtube.com/feeds/api/videos/" + id.ID + "?v=2");

                string xml = infoURL.GetHTML(System.Text.Encoding.UTF8).Trim('\0');
                XDocument doc = XDocument.Load(new System.IO.StringReader(xml));

                title = doc == null ? string.Empty : doc.Root.Element(XName.Get("title", "http://www.w3.org/2005/Atom")).Value;

                titles.Add(id, title);
            }
            return title;
        }
    }
}
