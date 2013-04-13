using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class AlbumFactory<T> : AlbumFactory where T : Album
    {
        private Func<RawTrack, Artist, T> method;

        public AlbumFactory(Func<RawTrack, Artist, T> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            this.method = method;
        }

        public sealed override Album CreateAlbum(RawTrack trackinfo, Artist artist)
        {
            return method(trackinfo, artist);
        }
    }
    public class AlbumFactory
    {
        /// <summary>
        /// Internal constructor - use then generic type <see cref="AlbumFactory{T}"/> instead.
        /// </summary>
        internal AlbumFactory() { }

        public virtual Album CreateAlbum(RawTrack trackinfo, Artist artist)
        {
            return new Album(trackinfo, artist);
        }
    }
}
