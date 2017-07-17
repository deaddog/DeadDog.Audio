using System;
using System.IO;

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

        /// <summary>
        /// Creates a new instance of the <see cref="RawTrack"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new cloned instance of the current object.</returns>
        public object Clone()
        {
            return new RawTrack(Filepath, TrackTitle, AlbumTitle, TrackNumber, ArtistName, Year);
        }

        /// <summary>
        /// Creates a <see cref="RawTrack"/> from the specified <see cref="Stream"/>. Advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="input">A <see cref="Stream"/> that contains the data for this <see cref="RawTrack"/> at the position where reading shold start.</param>
        /// <returns>The <see cref="RawTrack"/> that this method creates.</returns>
        public static RawTrack FromStream(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (!input.CanRead)
                throw new ArgumentException("Input stream must support reading", nameof(input));

            var path = input.ReadString();
            var track = input.ReadString();
            var album = input.ReadString();
            int? number = input.ReadNullableInt32();
            var artist = input.ReadString();
            int? year = input.ReadNullableInt32();

            return new RawTrack(path, track, album, number, artist, year);
        }
        /// <summary>
        /// Saves this <see cref="RawTrack"/> to the specified <see cref="Stream"/>. Advances the position within the stream by the number of bytes written.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> where the <see cref="RawTrack"/> will be saved.</param>
        public void Save(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (!output.CanWrite)
                throw new ArgumentException("Output stream must support writing", nameof(output));

            output.Write(Filepath);
            output.Write(Filepath);
            output.Write(TrackTitle);
            output.Write(AlbumTitle);
            output.Write(TrackNumber);
            output.Write(ArtistName);
            output.Write(Year);
        }

        /// <summary>
        /// Should only be used in the static type constructor.
        /// </summary>
        private RawTrack()
        {
            Filepath = null;

            TrackTitle = null;
            AlbumTitle = "Unknown";
            ArtistName = "Unknown";
            TrackNumber = null;
            Year = null;
        }

        /// <summary>
        /// Initializes a new instance of the RawTrack class.
        /// </summary>
        /// <param name="filepath">The full path of the file.</param>
        /// <param name="tracktitle">The title of the track. Should be set to <c>null</c> if unknown.</param>
        /// <param name="albumtitle">The album name of the track. Should be set to <c>null</c> if unknown.</param>
        /// <param name="tracknumber">The tracknumber of the track on the album. Should be set to <c>null</c> if unknown.</param>
        /// <param name="artistname">The artistname for the track. Should be set to <c>null</c> if unknown.</param>
        /// <param name="year">The year the track was released. Should be set to <c>null</c> if unknown.</param>
        public RawTrack(string filepath, string tracktitle, string albumtitle, int? tracknumber, string artistname, int? year)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath), "filepath cannot equal null");

            Filepath = filepath;

            TrackTitle = string.IsNullOrWhiteSpace(tracktitle) ? null : tracktitle.Trim();
            AlbumTitle = string.IsNullOrWhiteSpace(albumtitle) ? null : albumtitle.Trim();
            ArtistName = string.IsNullOrWhiteSpace(artistname) ? null : artistname.Trim();

            if (tracknumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(tracknumber), "The track number cannot be negative or zero.");
            else
                TrackNumber = tracknumber;

            if (year <= 0)
                throw new ArgumentOutOfRangeException(nameof(year), "The release year cannot be negative or zero.");
            else
                Year = year;
        }

        /// <summary>
        /// Gets the title of the track. If the title is unknown, null is returned.
        /// </summary>
        public string TrackTitle { get; }
        /// <summary>
        /// Gets the album name of the track. If the title is unknown, null is returned.
        /// </summary>
        public string AlbumTitle { get; }
        /// <summary>
        /// Gets the tracknumber of the track on the album, or <c>null</c> if the tracknumber is unknown.
        /// </summary>
        public int? TrackNumber { get; }
        /// <summary>
        /// Gets the artistname for the track. If the artist is unknown, null is returned.
        /// </summary>
        public string ArtistName { get; }
        /// <summary>
        /// Gets the year the track was released, or <c>null</c> if the release year is unknown.
        /// </summary>
        public int? Year { get; }

        /// <summary>
        /// Gets the full path of the file.
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        /// Creates a human-readable string that represents this <see cref="RawTrack"/>.
        /// </summary>
        /// <returns>A string that represents this <see cref="RawTrack"/>.</returns>
        public override string ToString()
        {
            if (TrackTitle != null)
            {
                if (ArtistName != null)
                {
                    if (AlbumTitle != null)
                        return $"{ArtistName} [{AlbumTitle}] - {TrackTitle}";
                    else
                        return $"{ArtistName} - {TrackTitle}";
                }
                else
                    return TrackTitle;
            }
            else
                return Filepath;
        }

        public override int GetHashCode()
        {
            return Filepath.GetHashCode() ^
                ArtistName.GetHashCode() ^
                AlbumTitle.GetHashCode() ^
                TrackTitle.GetHashCode() ^
                TrackNumber.GetHashCode() ^
                Year.GetHashCode();
        }
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
            return Filepath.Equals(other.Filepath) &&
                ArtistName.Equals(other.ArtistName) &&
                AlbumTitle.Equals(other.AlbumTitle) &&
                TrackTitle.Equals(other.TrackTitle) &&
                TrackNumber.Equals(other.TrackNumber) &&
                Year.Equals(other.Year);
        }
    }
}
