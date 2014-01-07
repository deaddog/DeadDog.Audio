namespace DeadDog.Audio
{
    /// <summary>
    /// Represents the method that will handle the <see cref="IPlaylist.EntryChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="EntryChangedEventArgs"/> instance containing the event data.</param>
    public delegate void EntryChangedEventHandler(object sender, EntryChangedEventArgs e);
}
