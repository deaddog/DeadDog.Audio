﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Stores title, album, tracknumber and artist info for a track
    /// </summary>
    public class RawTrack : ICloneable, IEquatable<RawTrack>
    {
        private static RawTrack unknown;
        internal static RawTrack Unknown
        {
            get { return unknown; }
        }
        internal bool IsUnknown
        {
            get { return ReferenceEquals(this, unknown); }
        }

        static RawTrack()
        {
            unknown = new RawTrack();
        }

        internal static void SetNewArtistName(RawTrack item, string newname)
        {
            if (item == unknown)
                throw new ArgumentException("Cannot alter \"Unknown\"");

            item.artist = newname;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RawTrack"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new cloned instance of the current object.</returns>
        public object Clone()
        {
            RawTrack rt = new RawTrack();
            rt.album = this.album;
            rt.artist = this.artist;
            rt.file = this.file;
            rt.number = this.number;
            rt.track = this.track;
            return rt;
        }

        /// <summary>
        /// Creates a <see cref="RawTrack"/> from the specified <see cref="Stream"/>. Advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="input">A <see cref="Stream"/> that contains the data for this <see cref="RawTrack"/> at the position where reading shold start.</param>
        /// <returns>The <see cref="RawTrack"/> that this method creates.</returns>
        public static RawTrack FromStream(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.CanRead)
                throw new ArgumentException("Input stream must support reading", "input");

            IOAssistant io = new IOAssistant(input);
            return new RawTrack(io.ReadString(), io.ReadString(), io.ReadString(), io.ReadInt32(), io.ReadString(), io.ReadInt32());
        }
        /// <summary>
        /// Saves this <see cref="RawTrack"/> to the specified <see cref="Stream"/>. Advances the position within the stream by the number of bytes written.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> where the <see cref="RawTrack"/> will be saved.</param>
        public void Save(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException("output");
            if (!output.CanWrite)
                throw new ArgumentException("Output stream must support writing", "output");

            IOAssistant io = new IOAssistant(output);
            io.Write(this.file.FullName);
            io.Write(this.track);
            io.Write(this.album);
            io.Write(this.number);
            io.Write(this.artist);
            io.Write(this.year);
        }

        private System.IO.FileInfo file;
        private string track, album, artist;
        private int number, year;

        /// <summary>
        /// Should only be used in the static type constructor.
        /// </summary>
        private RawTrack()
        {
            this.file = null;

            this.track = null;
            this.album = "Unknown";
            this.number = TrackNumberIfUnknown;
            this.year = YearIfUnknown;

            this.artist = "Unknown";
        }

        /// <summary>
        /// Initializes a new instance of the RawTrack class.
        /// </summary>
        /// <param name="filepath">The full path of the file.</param>
        /// <param name="tracktitle">The title of the track. Should be set to null if unknown.</param>
        /// <param name="albumtitle">The album name of the track. Should be set to null if unknown.</param>
        /// <param name="tracknumber">The tracknumber of the track on the album. Should be set to -1 if unknown.</param>
        /// <param name="artistname">The artistname for the track. Should be set to null if unknown.</param>
        public RawTrack(string filepath, string tracktitle, string albumtitle, int tracknumber, string artistname, int year)
        {
            if (filepath == null)
                throw new ArgumentNullException("filepath", "filepath cannot equal null");
            try
            {
                this.file = new System.IO.FileInfo(filepath);
            }
            catch (Exception e)
            {
                throw new ArgumentException("An error occured from the passed filepath", "filepath", e);
            }

            tracktitle = tracktitle == null ? null : tracktitle.Trim();
            track = tracktitle == "" ? null : tracktitle;

            albumtitle = albumtitle == null ? null : albumtitle.Trim();
            album = albumtitle == "" ? null : albumtitle;

            this.number = tracknumber;

            artistname = artistname == null ? null : artistname.Trim();
            artist = artistname == "" ? null : artistname;

            this.year = year;
        }

        /// <summary>
        /// Gets the title of the track. If the title is unknown, null is returned.
        /// </summary>
        public string TrackTitle
        {
            get { return track; }
        }
        /// <summary>
        /// Gets the album name of the track. If the title is unknown, null is returned.
        /// </summary>
        public string AlbumTitle
        {
            get { return album; }
        }
        /// <summary>
        /// Gets the tracknumber of the track on the album.
        /// </summary>
        public int TrackNumber
        {
            get { return number; }
        }
        /// <summary>
        /// Gets a boolean indicating whether the tracknumber is known.
        /// </summary>
        public bool TrackNumberUnknown
        {
            get { return number == TrackNumberIfUnknown; }
        }
        /// <summary>
        /// Gets the artistname for the track. If the artist is unknown, null is returned.
        /// </summary>
        public string ArtistName
        {
            get { return artist; }
        }
        public int Year
        {
            get { return year; }
        }
        public bool YearUnknown
        {
            get { return number == YearIfUnknown; }
        }

        /// <summary>
        /// Gets the value tracknumber should equal if it is unknown (-1).
        /// </summary>
        public const int TrackNumberIfUnknown = -1;
        public const int YearIfUnknown = -1;

        internal System.IO.FileInfo File
        {
            get { return file; }
        }
        /// <summary>
        /// Gets the filename (e.g. mysong.mp3)
        /// </summary>
        public string Filename
        {
            get { return file.Name; }
        }
        /// <summary>
        /// Gets the full path of the file.
        /// </summary>
        public string FullFilename
        {
            get { return file.FullName; }
        }

        /// <summary>
        /// Creates a human-readable string that represents this <see cref="RawTrack"/>.
        /// </summary>
        /// <returns>A string that represents this <see cref="RawTrack"/>.</returns>
        public override string ToString()
        {
            if (track != null)
            {
                if (artist != null && album != null)
                    return artist + " [" + album + "] - " + track;
                else
                    return track;
            }
            else
                return file.FullName;
        }

        #region IEquatable<RawTrack> Members

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj is RawTrack)
                return this.Equals(obj as RawTrack);
            else
                return false;
        }

        public bool Equals(RawTrack other)
        {
            return this.file.FullName == other.file.FullName &&
                this.artist == other.artist &&
                this.album == other.album &&
                this.track == other.track &&
                this.number == other.number &&
                this.year == other.year;
        }

        #endregion
    }
}
