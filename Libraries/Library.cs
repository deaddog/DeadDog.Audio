using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class Library
    {
        private Artist.ArtistCollection artists;
        public Artist.ArtistCollection Artists
        {
            get { return artists; }
        }

        private Album.AlbumCollection albums;
        public Album.AlbumCollection Albums
        {
            get { return albums; }
        }

        private Track.TrackCollection tracks;
        public Track.TrackCollection Tracks
        {
            get { return tracks; }
        }

        public Library()
        {
            this.artists = new Artist.ArtistCollection();
            this.albums = new Album.AlbumCollection(null);
            this.tracks = new Track.TrackCollection();
        }

        public Track AddTrack(RawTrack track)
        {
            Artist artist = artists[track.ArtistName] ?? CreateArtist(track.ArtistName);
            Album album = albums[track.AlbumTitle] ?? CreateAlbum(track.AlbumTitle);

            throw new NotImplementedException();
        }

        private Artist CreateArtist(string artistname)
        {
            throw new NotImplementedException();
        }
        private Album CreateAlbum(string albumname)
        {
            throw new NotImplementedException();
        }
    }
}
