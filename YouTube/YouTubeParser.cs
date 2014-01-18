using DeadDog.Audio.Parsing;
using System;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public class YouTubeParser : IDataParser
    {
        private IDataParser fallbackParser;
        private string documentPath;

        public YouTubeParser(string directory, IDataParser parser)
        {
            this.fallbackParser = parser;

            System.IO.FileInfo file = new System.IO.FileInfo(XML.DocumentPath(directory));
            if (!file.Exists)
                documentPath = null;
            else
                documentPath = file.FullName;
        }

        public RawTrack ParseTrack(string filepath)
        {
            if (documentPath == null)
                return null;

            throw new NotImplementedException();
        }
    }
}
