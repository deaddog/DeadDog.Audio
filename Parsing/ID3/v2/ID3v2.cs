using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    public class ID3v2
    {
        private TagHeader header;
        private string title = null;
        private string artist = null;
        private string album = null;
        private string year = null;
        private string trackstring = null;
        private int tracknumber = -1;

        private int frameCount;

        public ID3v2(System.IO.Stream stream)
        {
            using (FrameReader reader = new FrameReader(stream))
            {
                setvalues(reader);
            }
        }
        public ID3v2(string filename)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            using (FrameReader reader = new FrameReader(fs))
            {
                setvalues(reader);
            }
        }
        private void setvalues(FrameReader reader)
        {
            this.frameCount = reader.FrameCount;
            this.header = reader.TagHeader;
            this.title = reader.Read<string>("TIT2", ReadString, null);
            this.artist = reader.Read<string>("TPE1", ReadString, null);
            this.album = reader.Read<string>("TALB", ReadString, null);
            this.year = reader.Read<string>("TYER", ReadString, null);
            this.trackstring = reader.Read<string>("TRCK", ReadString, null);

            if (trackstring == null)
            {
                this.tracknumber = -1;
            }
            else if (trackstring.Contains("/"))
            {
                string s = trackstring.Substring(0, trackstring.IndexOf('/'));

                this.tracknumber = -1;
                int.TryParse(s, out this.tracknumber);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < trackstring.Length; i++)
                {
                    if (char.IsDigit(trackstring[i]))
                        sb.Append(trackstring[i]);
                }
                this.tracknumber = -1;
                int.TryParse(sb.ToString(), out this.tracknumber);
            }
        }

        public bool TagFound
        {
            get { return !TagHeader.IsEmpty(header) && frameCount > 0; }
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
        public string TrackString
        {
            get { return trackstring; }
        }
        public int TrackNumber
        {
            get { return tracknumber; }
        }
        public string Year
        {
            get { return year; }
        }

        public static string ReadString(System.IO.BinaryReader reader)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding enc = null;
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            if (data.Length <= 1)
                return null;
            string s;
            switch (data[0])
            {
                case 0x00:
                    enc = iso;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                case 0x01:
                    if (data[1] == 0xFF && data[2] == 0xFE)
                        enc = Encoding.Unicode;
                    else if (data[1] == 0xFE && data[2] == 0xFF)
                        enc = Encoding.BigEndianUnicode;
                    else
                        throw new Exception("Unknown Encoding");

                    s = enc.GetString(data, 3, data.Length - 3); ;
                    break;
                case 0x02:
                    enc = Encoding.BigEndianUnicode;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                case 0x03:
                    enc = Encoding.UTF8;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                default:
                    throw new Exception("Unknown Encoding");
            }
            s = s.Trim('\0');
            return s;
        }
    }
}
