using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    /// <summary>
    /// Represents a method that handles the <see cref="DeadDog.Audio.Libraries.ArtistCollection.ArtistAdded"/> and the <see cref="DeadDog.Audio.Libraries.ArtistCollection.ArtistRemoved"/> events.
    /// </summary>
    /// <param name="collection">The artist collection.</param>
    /// <param name="e">The <see cref="ArtistEventArgs"/> instance containing the event data.</param>
    public delegate void ArtistEventHandler(ArtistCollection collection, ArtistEventArgs e);
}
