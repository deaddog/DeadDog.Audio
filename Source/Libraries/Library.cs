using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class Library : IEnumerable<Track>
    {
        private LibraryCollection<Artist> artists;
        public LibraryCollection<Artist> Artists
        {
            get { return artists; }
        }

        private LibraryCollection<Album> albums;
        public LibraryCollection<Album> Albums
        {
            get { return albums; }
        }

        private LibraryCollection<Track> tracks;
        public LibraryCollection<Track> Tracks
        {
            get { return tracks; }
        }

        private Dictionary<string, Track> trackDict;
        private Dictionary<Artist, int> artistTrackCount;

        public Library()
        {
            this.artists = new LibraryCollection<Artist>(LibraryComparisons.CompareArtistNames);
            this.albums = new LibraryCollection<Album>(LibraryComparisons.CompareArtistNamesAlbumTitles);
            this.tracks = new LibraryCollection<Track>(LibraryComparisons.CompareArtistNameAlbumTitlesTrackNumbers);

            this.trackDict = new Dictionary<string, Track>();
            this.artistTrackCount = new Dictionary<Artist, int>();
        }

        public Track AddTrack(RawTrack track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (trackDict.ContainsKey(track.Filepath))
                throw new ArgumentException(track.Filepath + " is already in library - use Update instead.", "track");

            Artist artist = GetOrCreateArtist(track.ArtistName);
            Album album = GetOrCreateAlbum(artist, track.AlbumTitle);

            Track t = new Track(track.Filepath, track.TrackTitle, track.TrackNumber, album, artist);
            trackDict.Add(track.Filepath, t);

            album.Tracks.Add(t);
            artist.Tracks.Add(t);
            tracks.Add(t);

            return t;
        }

        private Artist GetOrCreateArtist(string artistname)
        {
            Artist existing = artistname == null ?
                artists.FirstOrDefault(x => x.IsUnknown) :
                artists.FirstOrDefault(x => x.Name.Equals(artistname));

            if (existing != null)
                return existing;
            else
            {
                Artist artist = new Artist(artistname);
                artists.Add(artist);
                return artist;
            }
        }
        private Album GetOrCreateAlbum(Artist artist, string albumtitle)
        {
            Album existing = albumtitle == null ?
                artist.Albums.FirstOrDefault(x => x.IsUnknown) :
                albums.FirstOrDefault(x => x.Title.Equals(albumtitle));

            if (existing != null)
                return existing;
            else
            {
                Album album = new Album(albumtitle);
                albums.Add(album);
                return album;
            }
        }

        public Track UpdateTrack(RawTrack track)
        {
            if (!trackDict.TryGetValue(track.Filepath, out Track item))
                throw new ArgumentOutOfRangeException(nameof(track), "A track must be contained by a Library to be updated by it.");

            item.Title = track.TrackTitle;
            item.Tracknumber = track.TrackNumber;

            var oldAlbum = item.Album;
            var oldArtist = item.Artist;

            var updateAlbum = item.Album.Title != track.AlbumTitle;
            var updateArtist = item.Artist.Name != track.ArtistName;

            if (updateAlbum)
            {
                oldAlbum.Tracks.Remove(item);
                if (oldAlbum.Tracks.Count == 0)
                    albums.Remove(oldAlbum);
            }

            if (updateArtist)
            {
                oldArtist.Tracks.Remove(item);
                if (oldArtist.Tracks.Count == 0)
                    artists.Remove(oldArtist);

                Artist artist = GetOrCreateArtist(track.ArtistName);
                item.Artist = artist;
                artist.Tracks.Add(item);
            }

            if (updateAlbum)
            {
                Album album = GetOrCreateAlbum(item.Artist, track.AlbumTitle);
                item.Album = album;
                album.Tracks.Add(item);
            }

            return item;
        }

        public bool Contains(RawTrack track)
        {
            return trackDict.ContainsKey(track.Filepath);
        }

        public void RemoveTrack(Track track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (!trackDict.ContainsKey(track.FilePath))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            Artist artist = track.Artist;
            Album album = track.Album;

            tracks.Remove(track);
            trackDict.Remove(track.FilePath);

            album.Tracks.Remove(track);
            if (album.Tracks.Count == 0)
                albums.Remove(album);

            artist.Tracks.Remove(track);
            if (artist.Tracks.Count == 0)
                artists.Remove(artist);
        }
        public void RemoveTrack(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("track");

            if (!trackDict.TryGetValue(filename, out Track track))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            RemoveTrack(track);
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
