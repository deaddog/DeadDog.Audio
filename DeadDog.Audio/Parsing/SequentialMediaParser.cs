using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Provides methods for grouping implementations of <see cref="IMediaParser"/> that are tested sequentially untill succes.
    /// </summary>
    public class SequentialMediaParser : IMediaParser
    {
        private List<IMediaParser> _parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialMediaParser"/> class.
        /// </summary>
        public SequentialMediaParser()
        {
            _parsers = new List<IMediaParser>();
        }

        /// <summary>
        /// Adds a new parser to the <see cref="SequentialMediaParser"/>.
        /// This parser will be tested before all previously added parsers.
        /// </summary>
        /// <param name="parser">The parser that should be added to the <see cref="SequentialMediaParser"/>.</param>
        public void AddFirst(IMediaParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers.Insert(0, parser);
        }
        /// <summary>
        /// Adds a new parser to the <see cref="SequentialMediaParser"/>.
        /// This parser will be tested after all previously added parsers.
        /// </summary>
        /// <param name="parser">The parser that should be added to the <see cref="SequentialMediaParser"/>.</param>
        public void AddLast(IMediaParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers.Add(parser);
        }

        /// <summary>
        /// Reads metadata from an audio-file by testing each contained <see cref="IMediaParser"/> in sequence.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <param name="track">
        /// When the method returns, contains the read metadata, if parsing succeeded, or null if parsing failed.
        /// The metadata will be returned from the first parser that succeeds, if any.
        /// </param>
        /// <returns><c>true</c> if the file was parsed successfully; otherwise, <c>false</c>.</returns>
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            for (int i = 0; i < _parsers.Count; i++)
                if (_parsers[i].TryParseTrack(filepath, out track))
                    return true;

            track = null;
            return false;
        }
    }
}
