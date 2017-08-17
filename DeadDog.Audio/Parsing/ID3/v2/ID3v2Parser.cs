using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    internal class ID3v2Parser : IMediaParser
    {
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                FrameReader reader;
                try
                {
                    reader = new FrameReader(fs);
                    track = GetTrack(filepath, reader);
                }
                catch
                {
                    track = null;
                    return false;
                }

                if (TagHeader.IsEmpty(reader.TagHeader) || reader.FrameCount == 0)
                {
                    track = null;
                    return false;
                }
            }

            return true;
        }

        private RawTrack GetTrack(string filepath, FrameReader reader)
        {
            var title = reader.ReadString("TIT2");
            var artist = reader.ReadString("TPE1");
            var album = reader.ReadString("TALB");
            var year = GetYear(reader);
            var tracknumber = GetTracknumber(reader);

            return new RawTrack(filepath, title, album, tracknumber, artist, year);
        }

        private int? GetYear(FrameReader reader)
        {
            var yearstring = reader.ReadString("TYER");
            if (yearstring == null)
                return null;
            else if (int.TryParse(yearstring, out int year))
                return year;
            else
                return null;
        }
        private int? GetTracknumber(FrameReader reader)
        {
            var trackstring = reader.ReadString("TRCK");

            if (trackstring == null)
                return null;
            else if (trackstring.Contains("/"))
            {
                string s = trackstring.Substring(0, trackstring.IndexOf('/'));
                if (int.TryParse(s, out var tracknumber))
                    return tracknumber;
                else
                    return null;
            }
            else
            {
                var sb = new StringBuilder();
                for (int i = 0; i < trackstring.Length; i++)
                {
                    if (char.IsDigit(trackstring[i]))
                        sb.Append(trackstring[i]);
                }

                if (int.TryParse(sb.ToString(), out var tracknumber))
                    return tracknumber;
                else
                    return null;
            }
        }
    }
}
