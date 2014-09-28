using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Artist
    {
        public class ArtistCollection : LibraryCollectionBase<Artist>
        {
            private Artist unknownArtist;

            internal ArtistCollection()
            {
                this.unknownArtist = new Artist(null);
            }

            public Artist UnknownArtist
            {
                get { return unknownArtist; }
            }
            internal override Artist _unknownElement
            {
                get { return unknownArtist; }
            }

            public event ArtistEventHandler ArtistAdded, ArtistRemoved;
            protected override void OnAdded(Artist element)
            {
                if (ArtistAdded != null)
                    ArtistAdded(this, new ArtistEventArgs(element));
            }
            protected override void OnRemoved(Artist element)
            {
                if (ArtistRemoved != null)
                    ArtistRemoved(this, new ArtistEventArgs(element));
            }

            protected override string GetName(Artist element)
            {
                return element.name;
            }
            protected override int Compare(Artist element1, Artist element2)
            {
                return element1.name.CompareTo(element2.name);
            }
        }
    }
}
