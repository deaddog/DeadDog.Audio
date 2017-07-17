using DeadDog.Audio.Parsing.ID3;

namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Exposes a simple media parser type.
    /// </summary>
    public static class MediaParser
    {
        /// <summary>
        /// Gets a <see cref="IMediaParser"/> that support all audio file types that can be parsed by the library.
        /// </summary>
        /// <param name="parseFilenames">if set to <c>true</c> the parser will attempt to parse meta data from file names using <see cref="FileNameMediaParser.GetDefault"/>.</param>
        /// <returns>A <see cref="IMediaParser"/> capable of handling all supported file types.</returns>
        public static IMediaParser GetDefault(bool parseFilenames)
        {
            ExtensionMediaParser extParser = new ExtensionMediaParser();

            extParser.Add("ogg", new OggParser());
            extParser.Add("mp3", new ID3Parser());
            extParser.Add("flac", new FlacParser());

            if (parseFilenames)
            {
                var parser = new SequentialMediaParser();

                parser.AddFirst(extParser);
                parser.AddLast(FileNameMediaParser.GetDefault());

                return parser;
            }
            else
                return extParser;
        }
    }
}
