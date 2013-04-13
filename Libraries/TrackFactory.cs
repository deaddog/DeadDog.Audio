using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public class TrackFactory<T> : TrackFactory where T : Track
    {
        private Func<RawTrack, Album, T> method;

        public TrackFactory(Func<RawTrack, Album, T> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            this.method = method;
        }

        public sealed override Track CreateTrack(RawTrack trackinfo, Album album)
        {
            return method(trackinfo, album);
        }
    }
    public abstract class TrackFactory
    {
        /// <summary>
        /// Internal constructor - use then generic type <see cref="TrackFactory{T}"/> instead.
        /// </summary>
        internal TrackFactory() { }

        public abstract Track CreateTrack(RawTrack trackinfo, Album album);
    }
}
