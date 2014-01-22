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

        public Library()
        {
            this.artists = new Artist.ArtistCollection();
            this.albums = new Album.AlbumCollection();
            this.tracks = new Track.TrackCollection();

            this.trackDict = new Dictionary<string, Track>();
        }

        public Track AddTrack(RawTrack track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (trackDict.ContainsKey(track.FullFilename))
                throw new ArgumentException(track.FullFilename + " is already in library - use Update instead.", "track");

            Artist artist = artists[track.ArtistName] ?? CreateArtist(track.ArtistName);
            Album album = albums[track.AlbumTitle] ?? CreateAlbum(track.AlbumTitle);

            Track t = new Track(track, album, artist);
            tracks.Add(t);
            album.Tracks.Add(t);

            if (album.IsUnknown && !artist.IsUnknown)
                artist.Albums.UnknownAlbum.Tracks.Add(t);

            trackDict.Add(track.FullFilename, t);

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

        public Track UpdateTrack(RawTrack track)
        {
            Track old;
            if (!trackDict.TryGetValue(track.FullFilename, out old))
                throw new ArgumentException(track.FullFilename + " was not found in library - use Add instead.", "track");

            old.Title = track.TrackTitle;
            old.Tracknumber = track.TrackNumberUnknown ? (int?)null : track.TrackNumber;
            
            Album album = albums[track.AlbumTitle] ?? CreateAlbum(track.AlbumTitle);
            if (album != old.Album)
            {
                old.Album.Tracks.Remove(old);
                if (old.Album.Tracks.Count == 0 && !old.Album.IsUnknown)
                    albums.Remove(old.Album);

                old.Album = album;
                album.Tracks.Add(old);
            }
            else
                album.Tracks.Reposition(old);

            Artist artist = artists[track.ArtistName] ?? CreateArtist(track.ArtistName);
            if (artist != old.Artist)
            {
                Artist oldArtist = old.Artist;
                old.Artist = artist;

                old.Album.Tracks.Remove(old);
                old.Album.Tracks.Add(old);
                if (oldArtist.Albums.Count == 0 && oldArtist.Albums.UnknownAlbum.Tracks.Count == 0)
                    artists.Remove(oldArtist);
            }

            return old;
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

            trackDict.Remove(track.FilePath);
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
