using DeadDog.Audio.Parsing;
using System;
using System.Xml.Linq;

namespace DeadDog.Audio.YouTube
{
    public class YouTubeParser : IDataParser
    {
        private IDataParser fallbackParser;
        private string documentPath;
        private XDocument document;

        public YouTubeParser(string xmlpath, IDataParser parser)
        {
            this.fallbackParser = parser;

            this.documentPath = xmlpath;
            System.IO.FileInfo file = new System.IO.FileInfo(xmlpath);
            if (!file.Exists)
                document = createNewXML();
            else
                document = XDocument.Load(xmlpath);
        }

        private static XDocument createNewXML()
        {
            return new XDocument(new XElement("tracks"));
        }

        public RawTrack ParseTrack(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
