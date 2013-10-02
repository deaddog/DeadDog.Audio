using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public partial class Artist
    {
        #region Properties

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

        #endregion

        public Artist(string name)
        {
            this.isunknown = name != null;
            this.albums = new Album.AlbumCollection();

            this.name = name ?? string.Empty;
        }
    }
}
