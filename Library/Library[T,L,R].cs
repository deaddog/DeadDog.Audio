using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    /// <summary>
    /// Represents a strongly typed collection of objects, defining a media-library. Requires specific implementations of <see cref="Track{T,L,R}"/>, <see cref="Album{T,L,R}"/>, <see cref="Artist{T,L,R}"/> and <see cref="LibraryFactory{T,L,R}"/> due to type-references.
    /// </summary>
    /// <typeparam name="T">The type of tracks in the library.</typeparam>
    /// <typeparam name="L">The type of albums in the library.</typeparam>
    /// <typeparam name="R">The type of artists in the library.</typeparam>
    public class Library<T, L, R> : IDisposable
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private Artist<T, L, R>.ArtistCollection artists;
        public Artist<T, L, R>.ArtistCollection Artists
        {
            get { return artists; }
        }

        private LibraryFactory<T, L, R> factory;
        internal LibraryFactory<T, L, R> Factory
        {
            get { return factory; }
        }

        public Library(LibraryFactory<T, L, R> factory)
        {
            this.factory = factory;

            R unknownArtist = factory.CreateArtist();
            unknownArtist.initialize(RawTrack.Unknown, this);

            this.artists = new Artist<T,L,R>.ArtistCollection(this, unknownArtist);
        }

        public T AddTrack(RawTrack trackinfo)
        {
            R artist = artists.GetArtist(trackinfo);
            if (artist == null)
                artist = artists.CreateArtist(factory, trackinfo);

            L album = artist.Albums.GetAlbum(trackinfo);
            if (album == null)
                album = artist.Albums.CreateAlbum(factory, trackinfo);

            T track = album.Tracks.CreateTrack(factory, trackinfo);

            return track;
        }

        public bool RemoveTrack(T track)
        {
            L album = track.Album;
            if (!album.Tracks.Contains(track))
                return false;
            R artist = album.Artist;
            if (!artist.Albums.Contains(album))
                return false;
            if (!artists.Contains(artist))
                return false;

            removeTrack(track);
            return true;
        }
        public bool RemoveTrack(RawTrack trackinfo)
        {
            R artist = artists.GetArtist(trackinfo);
            if (artist == null)
                return false;
            L album = artist.Albums.GetAlbum(trackinfo);
            if (album == null)
                return false;
            T track = album.Tracks.GetTrack(trackinfo);
            if (track == null)
                return false;

            removeTrack(track);
            return true;
        }
        private void removeTrack(T track)
        {
            L album = track.Album;
            R artist = album.Artist;

            track.destroy();
            album.Tracks.Remove(track);

            if (album.Tracks.Count == 0)
            {
                if (album != artist.Albums.UnknownAlbum)
                {
                    album.destroy();
                    artist.Albums.Remove(album);
                }

                if (artist.Albums.Count == 0 &&
                    artist.Albums.UnknownAlbum.Tracks.Count == 0 &&
                    artist != this.artists.UnknownArtist)
                {
                    artist.destroy();
                    artists.Remove(artist);
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Library{T,L,R}"/>.
        /// </summary>
        public void Dispose()
        {
            disposeArtistContent(artists.UnknownArtist);
            while (artists.Count > 0)
            {
                R artist = artists[0];
                disposeArtistContent(artist);
                artists.Remove(artist);
                artist.destroy();
            }
            artists = null;
        }
        private void disposeArtistContent(R artist)
        {
            disposeAlbumContent(artist.Albums.UnknownAlbum);
            while (artist.Albums.Count > 0)
            {
                L album = artist.Albums[0];
                disposeAlbumContent(album);
                artist.Albums.Remove(album);
                album.destroy();
            }
        }
        private void disposeAlbumContent(L album)
        {
            while (album.Tracks.Count > 0)
            {
                T track = album.Tracks[0];
                album.Tracks.Remove(track);
                track.destroy();
            }
        }
    }
}
