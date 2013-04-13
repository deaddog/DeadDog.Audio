using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class ArtistFactory<T> : ArtistFactory where T : Artist
    {
        private Func<RawTrack, Library, T> method;

        public ArtistFactory(Func<RawTrack, Library, T> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            this.method = method;
        }

        public sealed override Artist CreateArtist(RawTrack trackinfo, Library library)
        {
            return method(trackinfo, library);
        }
    }
    public abstract class ArtistFactory
    {
        /// <summary>
        /// Internal constructor - use then generic type <see cref="ArtistFactory{T}"/> instead.
        /// </summary>
        internal ArtistFactory() { }

        public virtual Artist CreateArtist(RawTrack trackinfo, Library library)
        {
            return new Artist(trackinfo, library);
        }
    }
}
