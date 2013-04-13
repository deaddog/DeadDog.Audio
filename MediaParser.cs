﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Parsing;
using DeadDog.Audio.Parsing.ID3;

namespace DeadDog.Audio
{
    public class MediaParser : IDataParser
    {
        private Dictionary<MediaTypes, IDataParser> parsers = new Dictionary<MediaTypes, IDataParser>();

        public MediaParser()
        {
            foreach (MediaTypes type in Enum.GetValues(typeof(MediaTypes)))
                parsers.Add(type, GetParser(type));
        }
        public MediaParser(MediaTypes types)
        {
            foreach (MediaTypes type in Enum.GetValues(typeof(MediaTypes)))
                if (types.HasFlag(type))
                    parsers.Add(type, GetParser(type));
        }

        private IDataParser GetParser(MediaTypes type)
        {
            switch (type)
            {
                case MediaTypes.Ogg: return new OggParser();
                case MediaTypes.Flac: return new FlacParser();
                case MediaTypes.Wma: return null;
                case MediaTypes.Mp3: return new ID3Parser();
                default:
                    throw new ArgumentException("Unknown media type.");
            }
        }

        public RawTrack ParseTrack(string filepath)
        {
            int dot = filepath.LastIndexOf('.');
            if (dot == -1)
                throw new Exception("No file extension.");

            string ext = filepath.Substring(dot + 1);
            MediaTypes type = getMediaType(ext);
            IDataParser parser = parsers[type];

            return parser.ParseTrack(filepath);
        }

        private MediaTypes getMediaType(string extension)
        {
            extension=extension.ToLower();
            switch (extension)
            {
                case "ogg": return MediaTypes.Ogg;
                case "flac": return MediaTypes.Flac;
                case "wma": return MediaTypes.Wma;
                case "mp3": return MediaTypes.Mp3;
                default:
                    throw new ArgumentException("Unknown media type.");
            }
        }
    }
}
