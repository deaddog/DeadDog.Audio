namespace DeadDog.Audio
{
    /// <summary>
    /// Represents the method that will handle the <see cref="IPlaylist{T}.EntryChanging"/> event.
    /// </summary>
    /// <param name="sender">The playable source of the event.</param>
    /// <param name="e">A <see cref="EntryChangedEventArgs"/> instance containing the event data.</param>
    public delegate void EntryChangingEventHandler<T>(IPlayable<T> sender, EntryChangingEventArgs<T> e);
}
