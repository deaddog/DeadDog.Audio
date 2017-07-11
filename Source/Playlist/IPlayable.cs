using System;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents an iteration on a collection of objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPlayable<T>
    {
        /// <summary>
        /// Gets the currently selected entry in the playable.
        /// </summary>
        T Entry { get; }

        /// <summary>
        /// Occurs after the <see cref="Entry"/> property is changed.
        /// </summary>
        event EventHandler EntryChanged;
        /// <summary>
        /// Occurs before the <see cref="Entry"/> property is changed.
        /// </summary>
        event EntryChangingEventHandler<T> EntryChanging;

        /// <summary>
        /// Moves to the next item in the playable.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();
    }
}
