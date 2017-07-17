using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadDog.Audio.Libraries
{
    public class Library
    {
        public LibraryCollection<Artist> Artists { get; }
        public LibraryCollection<Album> Albums { get; }
        public LibraryCollection<Track> Tracks { get; }

        private readonly Dictionary<string, Track> _trackDict;

        public Library()
        {
            Artists = new LibraryCollection<Artist>(LibraryComparisons.CompareArtistNames);
            Albums = new LibraryCollection<Album>(LibraryComparisons.CompareArtistNamesAlbumTitles);
            Tracks = new LibraryCollection<Track>(LibraryComparisons.CompareArtistNameAlbumTitlesTrackNumbers);

            _trackDict = new Dictionary<string, Track>();
        }

        public Track AddTrack(RawTrack track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (_trackDict.ContainsKey(track.Filepath))
                throw new ArgumentException(track.Filepath + " is already in library - use Update instead.", "track");

            Artist artist = GetOrCreateArtist(track.ArtistName);
            Album album = GetOrCreateAlbum(artist, track.AlbumTitle);
            
            Track t = new Track(track.Filepath, track.TrackTitle, track.TrackNumber, track.Year, album, artist);
            _trackDict.Add(track.Filepath, t);

            album.Tracks.Add(t);
            artist.Tracks.Add(t);
            Tracks.Add(t);

            return t;
        }

        private Artist GetOrCreateArtist(string artistname)
        {
            Artist existing = artistname == null ?
                Artists.FirstOrDefault(x => x.IsUnknown) :
                Artists.FirstOrDefault(x => x.Name.Equals(artistname));

            if (existing != null)
                return existing;
            else
            {
                Artist artist = new Artist(artistname);
                Artists.Add(artist);
                return artist;
            }
        }
        private Album GetOrCreateAlbum(Artist artist, string albumtitle)
        {
            Album existing = albumtitle == null ?
                artist.Albums.FirstOrDefault(x => x.IsUnknown) :
                Albums.FirstOrDefault(x => x.Title.Equals(albumtitle));

            if (existing != null)
                return existing;
            else
            {
                Album album = new Album(albumtitle);
                Albums.Add(album);
                return album;
            }
        }

        public Track UpdateTrack(RawTrack track)
        {
            if (!_trackDict.TryGetValue(track.Filepath, out Track item))
                throw new ArgumentOutOfRangeException(nameof(track), "A track must be contained by a Library to be updated by it.");

            item.Title = track.TrackTitle;
            item.Tracknumber = track.TrackNumber;
            item.Year = track.Year;

            var oldAlbum = item.Album;
            var oldArtist = item.Artist;

            var updateAlbum = item.Album.Title != track.AlbumTitle;
            var updateArtist = item.Artist.Name != track.ArtistName;

            if (updateAlbum)
            {
                oldAlbum.Tracks.Remove(item);
                if (oldAlbum.Tracks.Count == 0)
                    Albums.Remove(oldAlbum);
            }

            if (updateArtist)
            {
                oldArtist.Tracks.Remove(item);
                if (oldArtist.Tracks.Count == 0)
                    Artists.Remove(oldArtist);

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
            return _trackDict.ContainsKey(track.Filepath);
        }

        public void RemoveTrack(Track track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (!_trackDict.ContainsKey(track.FilePath))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            Artist artist = track.Artist;
            Album album = track.Album;

            Tracks.Remove(track);
            _trackDict.Remove(track.FilePath);

            album.Tracks.Remove(track);
            if (album.Tracks.Count == 0)
                Albums.Remove(album);

            artist.Tracks.Remove(track);
            if (artist.Tracks.Count == 0)
                Artists.Remove(artist);
        }
        public void RemoveTrack(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("track");

            if (!_trackDict.TryGetValue(filename, out Track track))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            RemoveTrack(track);
        }
    }
}
