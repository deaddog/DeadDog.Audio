using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract class LibraryFactory<T, L, R>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        public abstract T CreateTrack();
        public abstract L CreateAlbum();
        public abstract R CreateArtist();
    }
}
