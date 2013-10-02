using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Album
    {
        #region Properties

        private bool isunknown;
        public bool IsUnknown
        {
            get { return isunknown; }
        }

        private string title;
        public string Title
        {
            get { return title; }
        }

        private Track.TrackCollection tracks;
        public Track.TrackCollection Tracks
        {
            get { return tracks; }
        }

        // This is correct! - Artist should NOT be a constructor argument.
        private Artist artist = null;
        public Artist Artist
        {
            get { return artist; }
            internal set { artist = value; }
        }

        public bool HasArtist
        {
            get { return artist != null; }
        }

        #endregion

        public Album(string album)
        {
            this.isunknown = album == null;
            this.tracks = new Track.TrackCollection();

            this.title = album ?? string.Empty;
        }
    }
}
