﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio
{
    /// <summary>
    /// Represents a collection of objects that can be iterated through a selection of commands.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPlaylist<T>
    {
        /// <summary>
        /// Gets the currently seleected entry in the playlist.
        /// </summary>
        T Entry { get; }

        event EventHandler EntryChanged;
        event EntryChangingEventHandler<T> EntryChanging;

        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        /// <returns>true, if the move was successful; otherwise false.</returns>
        bool MoveNext();

        /// <summary>
        /// Resets the playlist.
        /// </summary>
        void Reset();
    }
}
