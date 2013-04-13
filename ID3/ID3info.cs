using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.ID3
{
    public class ID3info
    {
        private string artist;
        private string album;
        private string title;
        private string trackstring;
        private int tracknumber;

        private string searchstring;

        private System.IO.FileInfo file;
        private ID3v1 v1Info;
        private ID3v2 v2Info;

        public ID3info(Stream stream, string filename)
        {
            this.file = new FileInfo(filename);
            UpdateValues(stream);
        }
        public ID3info(string filename)
        {
            this.file = new FileInfo(filename);

            using (FileStream fs = new FileStream(filename, FileMode.Open))
                UpdateValues(fs);
        }
        /// <summary>
        /// internal - Initializes a new, and empty, instance of the <see cref="ID3info"/> class.
        /// </summary>
        internal ID3info()
        {
            this.artist = null;
            this.album = null;
            this.title = null;
            this.trackstring = null;
            this.tracknumber = -1;
            this.file = null;
            this.v1Info = null;
            this.v2Info = null;
        }
        /// <summary>
        /// internal - Initializes a new, and empty, instance of the <see cref="ID3info"/> class.
        /// </summary>
        /// <param name="file">internal - The <see cref="FileInfo"/> instance associated with this <see cref="ID3info"/>.</param>
        internal ID3info(FileInfo file)
            : this()
        {
            this.file = file;
        }

        public void UpdateValues()
        {
            if (!this.FileExists)
            {
                this.artist = null;
                this.album = null;
                this.title = null;
                this.trackstring = null;
                this.tracknumber = -1;
                this.v1Info = null;
                this.v2Info = null;

                return;
            }
            using (FileStream fs = new FileStream(this.file.FullName, FileMode.Open))
                UpdateValues(fs);
        }
        private void UpdateValues(Stream stream)
        {
            this.v1Info = new ID3v1(stream);
            this.v2Info = new ID3v2(stream);

            artist = longest(v2Info.Artist, v1Info.Artist);
            album = longest(v2Info.Album, v1Info.Album);
            title = longest(v2Info.Title, v1Info.Title);

            if (this.ID3v2.TrackNumber > 0 && ID3v1.TrackNumber > 0)
                trackstring = ID3v2.TrackString;
            else if (ID3v1.TrackNumber <= 0)
                trackstring = ID3v2.TrackString;
            else
                trackstring = ID3v1.TrackNumber.ToString("0");

            if (ID3v2.TrackNumber >= 0)
                tracknumber = ID3v2.TrackNumber;
            else
                tracknumber = ID3v1.TrackNumber;

            searchstring = (artist + " " + album + " " + title + file.Name.Substring(0, file.Name.LastIndexOf('.'))).ToLower();
            while (searchstring.Contains("  "))
                searchstring = searchstring.Replace("  ", " ");
            searchstring = searchstring.Trim();
        }

        public static ID3info Read(Stream input)
        {
            int length = readint(input);

            ID3info item = new ID3info();

            item.file = new FileInfo(readstring(input));
            item.artist = readstring(input);
            item.album = readstring(input);
            item.title = readstring(input);
            item.trackstring = readstring(input);
            item.tracknumber = readint(input);

            return item;
        }
        public static void Write(ID3info item, Stream output)
        {
            int length = byteCount(item.file.FullName)
                       + byteCount(item.artist)
                       + byteCount(item.album)
                       + byteCount(item.title)
                       + byteCount(item.trackstring)
                       + byteCount(item.tracknumber);

            write(output, length);
            write(output, item.file.FullName);
            write(output, item.artist);
            write(output, item.album);
            write(output, item.title);
            write(output, item.trackstring);
            write(output, item.tracknumber);
        }

        private static int byteCount(int value)
        {
            return 4;
        }
        private static int byteCount(string value)
        {
            return 4 + Encoding.UTF8.GetByteCount(value);
        }
        private static void write(Stream output, int value)
        {
            output.Write(BitConverter.GetBytes(value), 0, 4);
        }
        private static void write(Stream output, string value)
        {
            if (value == null)
                write(output, (int)-1);
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                write(output, buffer.Length);
                output.Write(buffer, 0, buffer.Length);
            }
        }
        private static int readint(Stream input)
        {
            byte[] buffer = new byte[4];
            input.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        private static string readstring(Stream input)
        {
            int length = readint(input);
            if (length == -1)
                return null;
            byte[] buffer = new byte[length];
            return Encoding.UTF8.GetString(buffer);
        }

        private string longest(string a, string b)
        {
            if (a == null)
                return b;
            if (b == null)
                return a;

            if (a.Length >= b.Length)
                return a;
            return b;
        }

        public ID3v2 ID3v2
        {
            get { return v2Info; }
        }
        public ID3v1 ID3v1
        {
            get { return v1Info; }
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

        public string FullFilename
        {
            get { return file.FullName; }
        }
        public string Filename
        {
            get { return file.Name; }
        }
        public bool FileExists
        {
            get { return file.Exists; }
        }

        /// <summary>
        /// Compares artist, album and title to the search string.
        /// </summary>
        /// <param name="search">The value to search for.</param>
        /// <returns>true if either artist, album or title contains the search string; false if neither does.</returns>
        public bool Match(string search)
        {
            return searchstring.Contains(search);
        }
        /// <summary>
        /// Compares artist, album and title to an array of search strings.
        /// </summary>
        /// <param name="search">The value(s) to search for.</param>
        /// <returns>false if one or more search strings were not found; true if all were.</returns>
        public bool MatchAll(params string[] search)
        {
            if(search.Length==0)
                return false;
            for (int i = 0; i < search.Length; i++)
                if (!this.searchstring.Contains(search[i]))
                    return false;
            return true;
        }
        /// <summary>
        /// Compares artist, album and title to an array of search strings.
        /// </summary>
        /// <param name="search">The value(s) to search for</param>
        /// <returns>true if one or more search strings were found; false if none were.</returns>
        public bool MatchAny(params string[] search)
        {
            if (search.Length == 0)
                return false;
            for (int i = 0; i < search.Length; i++)
                if (this.searchstring.Contains(search[i]))
                    return true;
            return false;
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }
}
