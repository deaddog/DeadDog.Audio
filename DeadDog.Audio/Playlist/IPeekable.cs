namespace DeadDog.Audio.Playlist
{
    /// <summary>
    /// Allows peeking to see the "next" element in a <see cref="IPlayable{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of elements handled by the peekable</typeparam>
    public interface IPeekablePlaylist<T> : IPlayable<T>
    {
        /// <summary>
        /// Tries to peek the next element in the playable.
        /// This will not affect the <see cref="IPlayable{T}.Entry"/> property.
        /// </summary>
        /// <param name="entry">When the method returns, this contains the next element in the playable.</param>
        /// <returns>true, if the peek was successful; otherwise false. This corresponds to the return value when calling <see cref="IPlayable{T}.MoveNext"/>.</returns>
        bool TryPeekNext(out T entry);
    }
}
