using System;

namespace DeadDog.Audio
{
    /// <summary>
    /// Provides data for the <see cref="IPlaylist{T}.EntryChanging"/> event.
    /// </summary>
    public class EntryChangingEventArgs<T> : EventArgs
    {
        private bool rejected;
        private bool canReject;

        private T entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryChangingEventArgs"/> class.
        /// </summary>
        public EntryChangingEventArgs(T entry, bool canReject)
        {
            this.rejected = false;
            this.canReject = canReject;
        }

        /// <summary>
        /// Attempts to reject the entry change.
        /// </summary>
        /// <returns><c>true</c> if the entry change could be rejected; otherwise, <c>false</c>.</returns>
        public bool RejectChange()
        {
            if (rejected)
                return true;

            if (canReject)
                rejected = true;

            return rejected;
        }

        /// <summary>
        /// Gets a value indicating whether the entry change is to be rejected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the entry change will be rejected; otherwise, <c>false</c>.
        /// </value>
        public bool Rejected
        {
            get { return rejected; }
        }

        /// <summary>
        /// Gets the new playlist entry. This can be null for reference types.
        /// </summary>
        public T NewEntry
        {
            get { return entry; }
        }
    }
}
