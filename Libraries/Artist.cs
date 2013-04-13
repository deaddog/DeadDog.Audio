using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Artist : IDisposable
    {
        private Library library;

        private bool isunknown;
        public bool IsUnknown
        {
            get { return isunknown; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private Album.AlbumCollection albums;
        public Album.AlbumCollection Albums
        {
            get { return albums; }
        }

        public Artist(RawTrack trackinfo, Library library)
        {
            this.library = library;

            Album unknown = library.albumFactory.CreateAlbum(RawTrack.Unknown, this);

            this.albums = new Album.AlbumCollection(this, unknown);
            this.isunknown = trackinfo.IsUnknown;

            this.name = trackinfo.ArtistName;
        }

        /// <summary>
        /// Removes the <see cref="Artist"/> from the associated library and releases resources.
        /// </summary>
        public void Remove()
        {
            if (this.IsUnknown)
                throw new InvalidOperationException("Cannot remove unknown artist.");

            while (albums.Count > 0)
                albums[0].Remove();
        }

        /// <summary>
        /// Called from <see cref="Album"/> - indicates that the album should be removed from the library.
        /// </summary>
        internal void RemoveAlbum(Album album)
        {
            bool disposeArtist = album.IsUnknown ? albums.Count == 0 : (albums.Count == 1 && albums.UnknownAlbum.Tracks.Count == 0);
            bool disposeAlbum = album.IsUnknown ? disposeArtist : true;

            if (disposeAlbum)
            {
                albums.RemoveAlbum(album);
                (album as IDisposable).Dispose();
            }

            if (disposeArtist)
                this.library.RemoveArtist(this);
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
