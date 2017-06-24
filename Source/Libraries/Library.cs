using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class Library : IEnumerable<Track>
    {
        private ArtistCollection artists;
        public ArtistCollection Artists
        {
            get { return artists; }
        }

        private AlbumCollection albums;
        public AlbumCollection Albums
        {
            get { return albums; }
        }

        private TrackCollection tracks;
        public TrackCollection Tracks
        {
            get { return tracks; }
        }

        private Dictionary<string, Track> trackDict;
        private Dictionary<Artist, int> artistTrackCount;

        public Library()
        {
            this.artists = new ArtistCollection();
            this.albums = new AlbumCollection();
            this.tracks = new TrackCollection();

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

            Track t = new Track(track);

            trackDict.Add(track.Filepath, t);
            addTrackToArtist(t, artist);
            addTrackToAlbum(t, album);
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
            Track item;
            if (!trackDict.TryGetValue(track.Filepath, out item))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be updated by it.");

            if (item.Title != track.TrackTitle)
                item.Title = track.TrackTitle;

            if (item.Album.Title != track.AlbumTitle)
            {
                removeTrackFromAlbum(item);
                var album = getAlbum(item.Artist, track.AlbumTitle);
                addTrackToAlbum(item, album);
            }

            if (item.Artist.Name != track.ArtistName)
            {
                removeTrackFromArtist(item);
                var artist = getArtist(track.ArtistName);
                addTrackToArtist(item, artist);
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

            removeTrackFromAlbum(track);
            removeTrackFromArtist(track);
        }
        public void RemoveTrack(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("track");

            Track track;
            if (!trackDict.TryGetValue(filename, out track))
                throw new ArgumentOutOfRangeException("track", "A track must be contained by a Library to be removed from it.");

            RemoveTrack(track);
        }

        private void addTrackToAlbum(Track track, Album album)
        {
            var artist = track.Artist;

            album.Tracks.Add(track);
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

            track.Album = album;
        }
        private void removeTrackFromAlbum(Track track)
        {
            var album = track.Album;

            album.Tracks.Remove(track);
            if (!album.IsUnknown)
            {
                if (album.Artist.IsUnknown)
                {
                    Artist a = album.Tracks[0].Artist;
                    for (int i = 1; i < album.Tracks.Count; i++)
                        if (a != album.Tracks[i].Artist)
                            a = artists.UnknownArtist;
                    if (album.Artist != a)
                    {
                        album.Artist.Albums.Remove(album);
                        a.Albums.Add(album);
                        album.Artist = a;
                    }
                }

                if (album.Tracks.Count == 0)
                {
                    //Remove album
                    albums.Remove(album);
                    if (!album.Artist.IsUnknown)
                    {
                        album.Artist.Albums.Remove(album);
                        album.Artist = null;
                    }
                }
            }
            track.Album = null;
        }

        private void addTrackToArtist(Track track, Artist artist)
        {
            track.Artist = artist;

            if (!artist.IsUnknown)
                artistTrackCount[artist]++;
        }
        private void removeTrackFromArtist(Track track)
        {
            var artist = track.Artist;

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
