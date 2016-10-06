using System;
using System.Collections.Generic;
using System.IO;

namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Provides methods for associating file extensions with specific implementations of <see cref="IMediaParser"/>.
    /// </summary>
    public class ExtensionMediaParser : IMediaParser
    {
        private Dictionary<string, IMediaParser> parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionMediaParser"/> class.
        /// </summary>
        public ExtensionMediaParser()
        {
            this.parsers = new Dictionary<string, IMediaParser>();
        }

        /// <summary>
        /// Adds a file extension and a parser to the <see cref="ExtensionMediaParser"/>.
        /// </summary>
        /// <param name="extension">The file extension (with or without a leading period).</param>
        /// <param name="parser">The parser associated with the extension.</param>
        public void Add(string extension, IMediaParser parser)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            extension = extension.TrimStart('.');

            parsers.Add(extension, parser);
        }

        /// <summary>
        /// Reads metadata from an audio-file using the <see cref="IMediaParser"/> associated with the files extension.
        /// </summary>
        /// <param name="filepath">The path of the file from which to read.</param>
        /// <returns>
        /// A <see cref="RawTrack"/> containing the parsed metadata, if parsing succeeded, or null if parsing failed.
        /// </returns>
        public RawTrack ParseTrack(string filepath)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath));

            string ext = Path.GetExtension(filepath);

            if (!parsers.ContainsKey(ext))
                return null;
            else
                return parsers[ext].ParseTrack(filepath);
        }
    }
}
