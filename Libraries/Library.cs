using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class Library : ITracks
    {
        private Artist.ArtistCollection artists;
        public Artist.ArtistCollection Artists
        {
            get { return artists; }
        }

        private TrackFactory trackFactory;
        internal AlbumFactory albumFactory;
        private ArtistFactory artistFactory;

        public Type TrackType
        {
            get { return trackFactory.GetType().GetGenericArguments()[0]; }
        }
        public Type AlbumType
        {
            get { return albumFactory.GetType().GetGenericArguments()[0]; }
        }
        public Type ArtistType
        {
            get { return artistFactory.GetType().GetGenericArguments()[0]; }
        }

        public Library()
            : this(
            new ArtistFactory<Artist>((r, l) => new Artist(r, l)),
            new AlbumFactory<Album>((r, a) => new Album(r, a)),
            new TrackFactory<Track>((r, a) => new Track(r, a)))
        {
        }
        public Library(ArtistFactory artistFactory, AlbumFactory albumFactory, TrackFactory trackFactory)
        {
            this.artistFactory = artistFactory;
            this.albumFactory = albumFactory;
            this.trackFactory = trackFactory;

            Artist unknownArtist = artistFactory.CreateArtist(RawTrack.Unknown, this);
            this.artists = new Artist.ArtistCollection(this, unknownArtist);
        }

        /// <summary>
        /// Called from <see cref="Artist"/> - indicates that the artist should be removed from the library.
        /// </summary>
        internal void RemoveArtist(Artist artist)
        {
            if (artist.IsUnknown)
                return;

            artists.RemoveArtist(artist);
            (artist as IDisposable).Dispose();
        }

        public Track AddTrack(RawTrack trackinfo)
        {
            Artist artist = artists[trackinfo];
            if (artist == null)
                artist = artists.CreateArtist(trackinfo, artistFactory);

            Album album = artist.Albums[trackinfo];
            if (album == null)
                album = artist.Albums.CreateAlbum(trackinfo, albumFactory);

            Track track = album.Tracks.CreateTrack(trackinfo, trackFactory);

            return track;
        }

        public IEnumerable<Track> GetTracks()
        {
            return artists.GetTracks();
        }
    }
}
