using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    internal class ID3v1
    {
        private static Encoding iso = Encoding.GetEncoding("ISO-8859-1");

        private bool tagfound;
        private string title = null;
        private string artist = null;
        private string album = null;
        private string year = null;
        private string comment = null;
        private int tracknumber = -1;

        private ID3v1()
        {
        }
        public ID3v1(Stream stream)
            : this(read128(stream))
        {
        }
        public ID3v1(string filename)
            : this(read128(filename))
        {
        }
        private ID3v1(byte[] buffer)
        {
            if (buffer.Length < 128)
            {
                this.tagfound = false;
                return;
            }
            else if (iso.GetString(buffer, 0, 3) != "TAG")
            {
                this.tagfound = false;
                return;
            }
            else
                this.tagfound = true;

            this.title = iso.GetString(buffer, 3, 30).Trim('\0', ' ');
            this.artist = iso.GetString(buffer, 33, 30).Trim('\0', ' ');
            this.album = iso.GetString(buffer, 63, 30).Trim('\0', ' ');
            this.year = iso.GetString(buffer, 93, 4).Trim('\0', ' ');

            bool hasTrack = (buffer[125] == 0);
            if (hasTrack)
            {
                comment = iso.GetString(buffer, 97, 28).Trim('\0', ' ');
                tracknumber = (int)buffer[126];
            }
            else
            {
                comment = iso.GetString(buffer, 97, 30).Trim('\0', ' ');
            }
        }

        private static byte[] read128(Stream stream)
        {
            if (stream.Length < 128)
                return new byte[0];

            long position = stream.Position;

            byte[] buffer = new byte[128];
            stream.Seek(-128, SeekOrigin.End);
            stream.Read(buffer, 0, 128);

            stream.Seek(position, SeekOrigin.Begin);

            return buffer;
        }
        private static byte[] read128(string filename)
        {
            byte[] buffer;
            using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                buffer = read128(stream);

            return buffer;
        }

        public bool TagFound
        {
            get { return tagfound; }
        }
        public string Artist
        {
            get { return artist; }
        }
        public string Album
        {
            get { return album; }
        }
        public string Title
        {
            get { return title; }
        }
        public int TrackNumber
        {
            get { return tracknumber; }
        }
        public string Year
        {
            get { return year; }
        }
    }
}
