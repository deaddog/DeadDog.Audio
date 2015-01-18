using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class Library : IEnumerable<Track>
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

        private Dictionary<string, Track> trackDict;
        private Dictionary<Artist, int> artistTrackCount;

        public Library()
        {
            this.artists = new Artist.ArtistCollection();
            this.albums = new Album.AlbumCollection();
            this.tracks = new Track.TrackCollection();

            this.trackDict = new Dictionary<string, Track>();
            this.artistTrackCount = new Dictionary<Artist, int>();
        }

        public Track AddTrack(RawTrack track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (trackDict.ContainsKey(track.FullFilename))
                throw new ArgumentException(track.FullFilename + " is already in library - use Update instead.", "track");

            Artist artist = getArtist(track.ArtistName);
            Album album = getAlbum(artist, track.AlbumTitle);

            Track t = new Track(track, album, artist);

            trackDict.Add(track.FullFilename, t);
            artistTrackCount[artist]++;

            tracks.Add(t);
            album.Tracks.Add(t);

            if (!album.IsUnknown)
            {
                if (album.Tracks.Count == 1)
                {
                    artist.Albums.Add(album);
                    album.Artist = artist;
                }
                else if (album.Artist != artist && album.Artist != artists.UnknownArtist)
                {
                    album.Artist.Albums.Remove(album);
                    artists.UnknownArtist.Albums.Add(album);
                    album.Artist = artists.UnknownArtist;
                }
            }

            return t;
        }
        private Artist getArtist(string artistname)
        {
            if (artistname == null || artistname.Length == 0)
                return artists.UnknownArtist;
            else if (artists.Contains(artistname))
                return artists[artistname];
            else
            {
                Artist artist = new Artist(artistname);
                artists.Add(artist);
                artistTrackCount.Add(artist, 0);
                return artist;
            }
        }
        private Album getAlbum(Artist artist, string albumname)
        {
            if (albumname == null || albumname.Length == 0)
                return artist.Albums.UnknownAlbum;
            else if (albums.Contains(albumname))
                return albums[albumname];
            else
            {
                Album album = new Album(albumname);
                albums.Add(album);
                return album;
            }
        }

        public Track UpdateTrack(RawTrack track)
        {
            if (!trackDict.ContainsKey(track.FullFilename))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            throw new NotImplementedException();
        }

        public bool Contains(RawTrack track)
        {
            return trackDict.ContainsKey(track.FullFilename);
        }

        public void RemoveTrack(Track track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (!trackDict.ContainsKey(track.FilePath))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            Artist artist = track.Artist;
            Album album = track.Album;

            trackDict.Remove(track.FilePath);
            if (!artist.IsUnknown)
            {
                artistTrackCount[artist]--;
                if (artistTrackCount[artist] == 0)
                {
                    //Remove artist
                    artists.Remove(artist);
                    artistTrackCount.Remove(artist);
                }
            }
            track.Artist = null;

            tracks.Remove(track);
            album.Tracks.Remove(track);
            if (!album.IsUnknown)
            {
                if (album.Tracks.Count == 0)
                {
                    //Remove album
                    throw new NotImplementedException();
                }
                else if (album.Tracks.Count == 1)
                {
                    artist.Albums.Add(album);
                    album.Artist = artist;
                }
                else if (album.Artist != artist && album.Artist != artists.UnknownArtist)
                {
                    album.Artist.Albums.Remove(album);
                    artists.UnknownArtist.Albums.Add(album);
                    album.Artist = artists.UnknownArtist;
                }
            }
            track.Album = null;
        }
        public void RemoveTrack(string filename)
        {
            RemoveTrack(trackDict[filename]);
        }

        IEnumerator<Track> IEnumerable<Track>.GetEnumerator()
        {
            foreach (var artist in artists)
                foreach (var album in artist.Albums)
                    foreach (var track in album.Tracks)
                        yield return track;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<Track>).GetEnumerator();
        }
    }
}
