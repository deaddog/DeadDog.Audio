using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    /// <summary>
    /// Represents a method that handles the <see cref="DeadDog.Audio.Libraries.Album.AlbumCollection.AlbumAdded"/> and the <see cref="DeadDog.Audio.Libraries.Album.AlbumCollection.AlbumRemoved"/> events.
    /// </summary>
    /// <param name="collection">The album collection.</param>
    /// <param name="e">The <see cref="AlbumEventArgs"/> instance containing the event data.</param>
    public delegate void AlbumEventHandler(Album.AlbumCollection collection, AlbumEventArgs e);
}
