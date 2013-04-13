using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    /// <summary>
    /// Serves as the base type for tracks, albums and artists within the <see cref="Library{T,L,R}"/>.
    /// </summary>
    /// <typeparam name="O">When used in a derived class, specifies this types owner type.</typeparam>
    public abstract class LibraryItem<O> where O : class
    {
        internal LibraryItem()
        {
            isInitialized = false;
            isDestroyed = false;
        }

        private bool isInitialized;
        /// <summary>
        /// Gets whether this <see cref="LibraryItem{O}"/> is fully initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return isInitialized; }
        }
        /// <summary>
        /// When overridden in a derived class, performs type-specific initialization of this <see cref="LibraryItem{O}"/>.
        /// </summary>
        protected virtual void Initialize()
        {
        }
        internal abstract void innerInitialize(RawTrack trackinfo, O owner);

        internal void initialize(RawTrack trackinfo, O owner)
        {
            innerInitialize(trackinfo, owner);
            this.Initialize();
            isInitialized = true;
        }

        private bool isDestroyed;
        /// <summary>
        /// Gets whether this <see cref="LibraryItem{O}"/> is fully destroyed.
        /// </summary>
        public bool IsDestroyed
        {
            get { return isDestroyed; }
        }
        /// <summary>
        /// When overridden in a derived class, performs type-specific destruction of this <see cref="LibraryItem{O}"/>.
        /// </summary>
        protected virtual void Destroy()
        {
        }
        internal abstract void innerDestroy();

        internal void destroy()
        {
            isDestroyed = true;
            this.Destroy();
            this.innerDestroy();
        }
    }
}
