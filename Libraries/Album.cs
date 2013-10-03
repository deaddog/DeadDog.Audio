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
            this.tracks = new Track.TrackCollection(trackAdded, trackRemoved);

            this.title = album ?? string.Empty;
        }

        private void trackAdded(Track.TrackCollection collection, TrackEventArgs e)
        {
            if (collection != tracks)
                throw new InvalidOperationException("Album attempted to alter wrong trackcollection.");

            if (collection.Count == 1)
                this.artist = e.Track.Artist;
            else if (e.Track.Artist != null && e.Track.Artist != this.artist)
                this.artist = null;
        }
        private void trackRemoved(Track.TrackCollection collection, TrackEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
