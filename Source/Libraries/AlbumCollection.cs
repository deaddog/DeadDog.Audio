using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Album
    {
        public class AlbumCollection : LibraryCollectionBase<Album>
        {
            private Album unknownAlbum;

            internal AlbumCollection()
            {
                this.unknownAlbum = new Album(null);
            }

            public Album UnknownAlbum
            {
                get { return unknownAlbum; }
            }
            internal override Album _unknownElement
            {
                get { return unknownAlbum; }
            }

            public event AlbumEventHandler AlbumAdded, AlbumRemoved;
            protected override void OnAdded(Album element)
            {
                if (AlbumAdded != null)
                    AlbumAdded(this, new AlbumEventArgs(element));
            }
            protected override void OnRemoved(Album element)
            {
                if (AlbumRemoved != null)
                    AlbumRemoved(this, new AlbumEventArgs(element));
            }

            protected override string GetName(Album element)
            {
                return element.title;
            }
            protected override int Compare(Album element1, Album element2)
            {
                return element1.title.CompareTo(element2.title);
            }
        }
    }
}
