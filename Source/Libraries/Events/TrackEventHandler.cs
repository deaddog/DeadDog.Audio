using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    /// <summary>
    /// Represents a method that handles the <see cref="DeadDog.Audio.Libraries.TrackCollection.TrackAdded"/> and the <see cref="DeadDog.Audio.Libraries.TrackCollection.TrackRemoved"/> events.
    /// </summary>
    /// <param name="collection">The track collection.</param>
    /// <param name="e">The <see cref="TrackEventArgs"/> instance containing the event data.</param>
    public delegate void TrackEventHandler(TrackCollection collection, TrackEventArgs e);
}
