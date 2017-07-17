using System;
using System.Collections.Generic;

namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Provides methods for grouping implementations of <see cref="IMediaParser"/> that are tested sequentially untill succes.
    /// </summary>
    public class SequentialMediaParser : IMediaParser
    {
        private List<IMediaParser> parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialMediaParser"/> class.
        /// </summary>
        public SequentialMediaParser()
        {
            this.parsers = new List<IMediaParser>();
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

            parsers.Insert(0, parser);
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

            parsers.Add(parser);
        }

        /// <summary>
        /// Reads metadata from an audio-file by testing each contained <see cref="IMediaParser"/>.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <returns>
        /// A <see cref="RawTrack"/> containing the parsed metadata, if parsing succeeded, or null if parsing failed.
        /// The metadata will be returned from the first parser that succeeds, if any.
        /// </returns>
        public RawTrack ParseTrack(string filepath)
        {
            RawTrack track;

            for (int i = 0; i < parsers.Count; i++)
                if (parsers[i].TryParseTrack(filepath, out track))
                    return track;

            return null;
        }
    }
}
