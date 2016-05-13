using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Defines a parsing method that attempts to extract media information from a files name.
    /// </summary>
    public class FileNameMediaParser : IMediaParser
    {
        private List<Regex> regexes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNameMediaParser"/> class.
        /// </summary>
        public FileNameMediaParser()
        {
            this.regexes = new List<Regex>();
        }

        /// <summary>
        /// Gets a <see cref="FileNameMediaParser"/> with a set of predefined regular expressions.
        /// </summary>
        /// <returns>A <see cref="FileNameMediaParser"/> with a set of predefined regular expressions.</returns>
        public static FileNameMediaParser GetDefault()
        {
            var parser = new FileNameMediaParser();

            parser.Add("^(?<artist>[^-]+)-(?<title>[^-]+)$");
            parser.Add("^(?<artist>[^/]+)/(?<title>.+)$");

            return parser;
        }

        /// <summary>
        /// Adds a regular expression that should be checked against a file-name.
        /// See remark.
        /// </summary>
        /// <remarks>
        /// Data is extracted using named groups:
        ///  - 'artist' is the name of the track artist.
        ///  - 'album' is the name of the album on which the track is found.
        ///  - 'title' is the title of track.
        ///  - 'track' is the track number on the album.
        ///  - 'year' is the year the track was released.
        /// 
        /// All of the above are optional, except for title.
        /// </remarks>
        /// <param name="regex">The regular expresion that should be used to parse media information from filenames.</param>
        public void Add(Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            this.regexes.Add(regex);
        }
        /// <summary>
        /// Adds a regular expression that should be checked against a file-name.
        /// See remark on <see cref="Add(Regex)"/>.
        /// </summary>
        /// <param name="regex">The regular expresion that should be used to parse media information from filenames.</param>
        public void Add(string regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            Add(new Regex(regex));
        }

        /// <summary>
        /// Attempts to extract metadata from an audio-files filename.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <returns>
        /// A <see cref="RawTrack"/> containing the parsed metadata, if parsing succeeded, or null if parsing failed.
        /// </returns>
        public RawTrack ParseTrack(string filepath)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath));

            string filename = Path.ChangeExtension(Path.GetFileName(filepath), null);

            Match match;

            for (int i = 0; i < regexes.Count; i++)
                if ((match = regexes[i].Match(filename)).Success)
                {
                    string artist = match.Groups["artist"].Value.Trim();
                    string album = match.Groups["album"].Value.Trim();
                    string title = match.Groups["title"].Value.Trim();
                    string trackStr = match.Groups["track"].Value.Trim();
                    string yearStr = match.Groups["year"].Value.Trim();

                    if (title == "")
                        continue;

                    int track;
                    if (int.TryParse(trackStr, out track))
                        track = RawTrack.TrackNumberIfUnknown;

                    int year;
                    if (int.TryParse(yearStr, out year))
                        year = RawTrack.YearIfUnknown;

                    return new RawTrack(filepath, title, album, track, artist, year);
                }

            return null;
        }
    }
}
