using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Track : IDisposable
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

        private bool hasnumber;
        public bool HasTracknumber
        {
            get { return hasnumber; }
        }
        private int tracknumber;
        public int Tracknumber
        {
            get { return tracknumber; }
        }

        private Album album;
        public Album Album
        {
            get { return album; }
        }

        private string albumtitle;
        public string AlbumTitle
        {
            get { return albumtitle; }
        }

        private string artistname;
        public string ArtistName
        {
            get { return artistname; }
        }

        #endregion

        public Track(RawTrack trackinfo, Album album)
        {
            this.file = trackinfo.File;
            this.album = album;
            this.albumtitle = trackinfo.AlbumTitle;
            this.artistname = trackinfo.ArtistName;
            this.title = trackinfo.TrackTitle;
            this.hasnumber = !trackinfo.TrackNumberUnknown;
            this.tracknumber = trackinfo.TrackNumber;
        }

        /// <summary>
        /// Removes the <see cref="Track"/> from the associated library and releases resources.
        /// </summary>
        public void Remove()
        {
            this.album.RemoveTrack(this);
        }

        private static string ToLowerOrNull(string input)
        {
            if (input == null)
                return null;
            else
                return input.ToLower();
        }

        protected virtual void Dispose()
        {
        }
        void IDisposable.Dispose()
        {
            this.Dispose();
        }
    }
}
