using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Album : IDisposable
    {
        private bool isunknown;
        public bool IsUnknown
        {
            get { return isunknown; }
        }

        private string artistname;
        public string ArtistName
        {
            get { return artistname; }
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
        private Artist artist;
        public Artist Artist
        {
            get { return artist; }
        }

        public Album(RawTrack trackinfo, Artist artist)
        {
            this.isunknown = trackinfo.IsUnknown;
            this.tracks = new Track.TrackCollection(this);

            this.title = trackinfo.AlbumTitle;

            this.artistname = trackinfo.ArtistName;
            this.artist = artist;
        }

        /// <summary>
        /// Removes the <see cref="Album"/> from the associated library and releases resources.
        /// </summary>
        public void Remove()
        {
            if (this.IsUnknown)
                throw new InvalidOperationException("Cannot remove unknown album.");

            while (tracks.Count > 0)
                tracks[0].Remove();
        }

        /// <summary>
        /// Called from <see cref="Track"/> - indicates that the track should be removed from the library.
        /// </summary>
        internal void RemoveTrack(Track track)
        {
            tracks.RemoveTrack(track);
            (track as IDisposable).Dispose();

            if (tracks.Count == 0)
                this.artist.RemoveAlbum(this);
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
