using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    internal class ID3v1
    {
        private static Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");

        public bool TagFound { get; }
        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }
        public int? TrackNumber { get; }
        public int? Year { get; }

        private ID3v1(bool tagFound, string artist, string album, string title, int? trackNumber, int? year)
        {
            TagFound = tagFound;
            Artist = artist;
            Album = album;
            Title = title;
            TrackNumber = trackNumber;
            Year = year;
        }

        public static ID3v1 FromFile(string filepath)
        {
            using (Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
                return FromStream(stream);
        }
        public static ID3v1 FromStream(Stream stream)
        {
            if (stream.Length < 128)
                return FromTagBytes(new byte[0]);

            long position = stream.Position;

            byte[] buffer = new byte[128];
            stream.Seek(-128, SeekOrigin.End);
            stream.Read(buffer, 0, 128);

            stream.Seek(position, SeekOrigin.Begin);

            return FromTagBytes(buffer);
        }

        private static ID3v1 FromTagBytes(byte[] tag)
        {
            if (tag.Length < 128)
                return new ID3v1(false, null, null, null, null, null);

            if (_encoding.GetString(tag, 0, 3) != "TAG")
                return new ID3v1(false, null, null, null, null, null);

            var title = _encoding.GetString(tag, 3, 30).Trim('\0', ' ');
            var artist = _encoding.GetString(tag, 33, 30).Trim('\0', ' ');
            var album = _encoding.GetString(tag, 63, 30).Trim('\0', ' ');
            var year = GetYear(tag);
            var tracknumber = GetTracknumber(tag);

            return new ID3v1(true, artist, album, title, tracknumber, year);
        }

        private static int? GetYear(byte[] tag)
        {
            var yearStr = _encoding.GetString(tag, 93, 4).Trim('\0', ' ');

            if (int.TryParse(yearStr, out int year))
                return year;
            else
                return null;
        }
        private static int? GetTracknumber(byte[] tag)
        {
            bool hasTrack = (tag[125] == 0);
            if (hasTrack)
                return tag[126];
            else
                return null;
        }
    }
}
