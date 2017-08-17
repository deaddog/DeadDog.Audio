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
        private Dictionary<string, IMediaParser> _parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionMediaParser"/> class.
        /// </summary>
        public ExtensionMediaParser()
        {
            _parsers = new Dictionary<string, IMediaParser>();
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

            _parsers.Add(extension, parser);
        }

        /// <summary>
        /// Reads metadata from an audio-file using the <see cref="IMediaParser"/> associated with the files extension.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <param name="track">
        /// When the method returns, contains the read metadata, if parsing succeeded, or null if parsing failed.
        /// </param>
        /// <returns><c>true</c> if the file was parsed successfully; otherwise, <c>false</c>.</returns>
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath));

            string ext = Path.GetExtension(filepath).TrimStart('.');

            if (!_parsers.ContainsKey(ext))
            {
                track = null;
                return false;
            }
            else
                return _parsers[ext].TryParseTrack(filepath, out track);
        }
    }
}
