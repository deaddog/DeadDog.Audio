using System;
using System.Text.RegularExpressions;

namespace DeadDog.Audio.Parsing.ID3
{
    /// <summary>
    /// Provides a method for reading metadata from files with an ID3 tag, using an <see cref="ID3info"/> object.
    /// </summary>
    public class ID3Parser : IMediaParser
    {
        /// <summary>
        /// Reads metadata from files with an ID3 tag, using an <see cref="ID3info"/> object.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <returns>A <see cref="RawTrack"/> containing the parsed metadata, if parsing succeeded. If parsing fails an exception is thrown.</returns>
        public RawTrack ParseTrack(string filepath)
        {
            var v1Info = new ID3v1(filepath);
            var v2Info = new ID3v2(filepath);

            if (!v1Info.TagFound && !v2Info.TagFound)
                throw new Exception("No tags found.");

            var artist = Longest(v2Info.Artist, v1Info.Artist);
            var album = Longest(v2Info.Album, v1Info.Album);
            var title = Longest(v2Info.Title, v1Info.Title);

            var tracknumber = GetTrackNumber(v1Info, v2Info);
            var year = GetYear(v1Info, v2Info);

            return new RawTrack(filepath, title, album, tracknumber, artist, year);
        }

        private string Longest(string a, string b)
        {
            if (a == null)
                return b;
            if (b == null)
                return a;

            if (a.Length >= b.Length)
                return a;
            return b;
        }

        private int? GetTrackNumber(ID3v1 v1, ID3v2 v2)
        {
            if (v2.TrackNumber > 0)
                return v2.TrackNumber;
            else if (v1.TrackNumber > 0)
                return v1.TrackNumber;
            else
                return null;
        }
        private int? GetYear(ID3v1 v1, ID3v2 v2)
        {
            if (v2.Year > 0)
                return v2.Year;
            else if (v1.Year != null)
            {
                Match mYear = Regex.Match(v1.Year, "[0-9]{4,4}");
                if (mYear.Success && int.Parse(mYear.Value) > 0)
                    return int.Parse(mYear.Value);
                else
                    return null;
            }
            else
                return null;
        }
    }
}
