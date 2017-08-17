using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    internal class ID3v1Parser : IMediaParser
    {
        private static Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");

        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            var tagBytes = GetTagBytes(filepath);

            if (tagBytes.Length < 128 || _encoding.GetString(tagBytes, 0, 3) != "TAG")
            {
                track = null;
                return false;
            }

            var title = _encoding.GetString(tagBytes, 3, 30).Trim('\0', ' ');
            var artist = _encoding.GetString(tagBytes, 33, 30).Trim('\0', ' ');
            var album = _encoding.GetString(tagBytes, 63, 30).Trim('\0', ' ');
            var year = GetYear(tagBytes);
            var tracknumber = GetTracknumber(tagBytes);

            track = new RawTrack(filepath, title, album, tracknumber, artist, year);
            return true;
        }

        private byte[] GetTagBytes(string filepath)
        {
            byte[] buffer = new byte[128];

            using (Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                if (stream.Length < 128)
                    return new byte[0];

                stream.Read(buffer, 0, 128);
            }

            return buffer;
        }

        private int? GetYear(byte[] tag)
        {
            var yearStr = _encoding.GetString(tag, 93, 4).Trim('\0', ' ');

            if (int.TryParse(yearStr, out int year))
                return year;
            else
                return null;
        }
        private int? GetTracknumber(byte[] tag)
        {
            bool hasTrack = (tag[125] == 0);
            if (hasTrack)
                return tag[126];
            else
                return null;
        }
    }
}
