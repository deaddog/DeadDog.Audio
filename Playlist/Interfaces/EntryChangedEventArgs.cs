using System;

namespace DeadDog.Audio
{
    /// <summary>
    /// Provides data for the <see cref="IPlaylist{T}.EntryChanged"/> event.
    /// </summary>
    public class EntryChangedEventArgs : EventArgs
    {
        private bool changeAccepted;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryChangedEventArgs"/> class.
        /// </summary>
        public EntryChangedEventArgs()
        {
            this.changeAccepted = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entry change is accepted; such as whether the new entry exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the entry change is accepted; otherwise, <c>false</c>.
        /// </value>
        public bool ChangeAccepted
        {
            get { return changeAccepted; }
            set { changeAccepted = value; }
        }
    }
}
