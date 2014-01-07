using System;

namespace DeadDog.Audio
{
    /// <summary>
    /// Provides data for the <see cref="IPlaylist{T}.EntryChanged"/> event.
    /// </summary>
    public class EntryChangedEventArgs : EventArgs
    {
        private bool rejected;
        private bool canReject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryChangedEventArgs"/> class.
        /// </summary>
        public EntryChangedEventArgs(bool canReject)
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
    }
}
