using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    /// <summary>
    /// Provides data for the <see cref="DeadDog.Audio.Libraries.ArtistCollection.ArtistAdded"/> and the <see cref="DeadDog.Audio.Libraries.ArtistCollection.ArtistRemoved"/> events.
    /// </summary>
    public class ArtistEventArgs : EventArgs
    {
        private Artist artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistEventArgs"/> class.
        /// </summary>
        /// <param name="artist">The artist.</param>
        public ArtistEventArgs(Artist artist)
        {
            this.artist = artist;
        }

        /// <summary>
        /// Gets the artist associated with the event.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        public Artist Artist
        {
            get { return artist; }
        }
    }
}
