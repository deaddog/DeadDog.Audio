using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Track
    {
        #region Fields and properties

        private System.IO.FileInfo file;
        public bool FileExist
        {
            get { file.Refresh(); return file.Exists; }
        }
        public string FilePath
        {
            get { return file.FullName; }
        }

        private string title;
        public string Title
        {
            get { return title; }
        }

        private int? tracknumber;
        public int? Tracknumber
        {
            get { return tracknumber; }
        }

        private Album album;
        public Album Album
        {
            get { return album; }
        }

        private Artist artist;
        public Artist Artist
        {
            get { return artist; }
        }

        #endregion

        public Track(RawTrack trackinfo, Album album, Artist artist)
        {
            this.file = trackinfo.File;
            this.album = album;
            this.artist = artist;
            this.title = trackinfo.TrackTitle;
            this.tracknumber = trackinfo.TrackNumberUnknown ?  (int?)null : trackinfo.TrackNumber;
        }
    }
}
