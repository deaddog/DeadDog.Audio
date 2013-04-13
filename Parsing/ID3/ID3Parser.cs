﻿using System;
using System.Collections.Generic;
using System.Text;

using DeadDog.Audio.ID3;

namespace DeadDog.Audio
{
    /// <summary>
    /// Provides a method for reading metadata from files with an ID3 tag, using an <see cref="ID3info"/> object.
    /// </summary>
    public class ID3Parser : ADataParser
    {
        /// <summary>
        /// Reads metadata from files with an ID3 tag, using an <see cref="ID3info"/> object.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <returns>A <see cref="RawTrack"/> containing the parsed metadata, if parsing succeeded. If parsing fails an exception is thrown.</returns>
        public override RawTrack ParseTrack(string filepath)
        {
            ID3info id3 = new ID3info(filepath);
            if (!id3.ID3v1.TagFound && !id3.ID3v2.TagFound)
                throw new Exception("No tags found.");

            return new RawTrack(filepath, id3.Title, id3.Album, id3.TrackNumber, id3.Artist);
        }
    }
}
