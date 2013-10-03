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
            this.albums = new Album.AlbumCollection();
            this.tracks = new Track.TrackCollection();
        }

        public Track AddTrack(RawTrack track)
        {
            Artist artist = artists[track.ArtistName] ?? CreateArtist(track.ArtistName);
            Album album = albums[track.AlbumTitle] ?? CreateAlbum(track.AlbumTitle);

            Track t = new Track(track, album, artist);
            tracks.Add(t);
            album.Tracks.Add(t);
            return t;
        }
        private Artist CreateArtist(string artistname)
        {
            Artist artist = new Artist(artistname);
            artists.Add(artist);
            return artist;
        }
        private Album CreateAlbum(string albumname)
        {
            Album album = new Album(albumname);
            albums.Add(album);
            return album;
        }

        public void RemoveTrack(Track track)
        {
            Album album = track.Album;
            Artist artist = track.Artist;

            tracks.Remove(track);
            if (album != null)
            {
                album.Tracks.Remove(track);
                if (album.Tracks.Count == 0 && !album.IsUnknown)
                    albums.Remove(album);
            }

            if (artist != null)
            {
                if (artist.Albums.Count == 0 && artist.Albums.UnknownAlbum.Tracks.Count == 0)
                    artists.Remove(artist);
            }
        }
    }
}
