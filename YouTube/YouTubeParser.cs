using DeadDog.Audio.Parsing;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public class YouTubeParser : IDataParser
    {
        private IDataParser fallbackParser;
        private string documentPath;
        private string directory;

        public YouTubeParser(string directory, IDataParser parser)
        {
            this.fallbackParser = parser;

            this.directory = Path.GetFullPath(directory);
            this.documentPath = XML.DocumentPath(directory);
        }

        public RawTrack ParseTrack(string filepath)
        {
            if (documentPath == null)
                return null;
            if (!new FileInfo(documentPath).Exists)
                return null;
            if (!filepath.StartsWith(directory))
                return null;

            XDocument document = XDocument.Load(documentPath);
            string title = getTitle(filepath, document);
            return parseTitle(filepath, title);
        }

        private string getTitle(string filepath, XDocument document)
        {
            string relative = filepath.Substring(directory.Length).TrimStart('\\');

            var tracks = from e in document.Element("tracks").Elements("track")
                         let path = e.Element("path").Value
                         where path == relative
                         let title = e.Element("title").Value
                         select title;

            return tracks.FirstOrDefault();
        }

        private RawTrack parseTitle(string filepath, string youtubeTitle)
        {
            Regex dash = new Regex("^(?<artist>[^-]+)-(?<track>[^-]+)$");
            Regex slash = new Regex("^(?<artist>[^/]+)/(?<track>.+)$");
            Match match;

            string title = youtubeTitle;
            string album = null;
            string artist = null;

            if ((match = dash.Match(youtubeTitle)).Success || (match = slash.Match(youtubeTitle)).Success)
            {
                artist = match.Groups["artist"].Value.Trim();
                title = match.Groups["track"].Value.Trim();
            }

            return new RawTrack(filepath, title, album, RawTrack.TrackNumberIfUnknown, artist, RawTrack.YearIfUnknown);
        }
    }
}
