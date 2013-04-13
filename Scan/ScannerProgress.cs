using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Scan
{
    /// <summary>
    /// Contains data describing the scanning progress of an <see cref="AudioScanner"/>.
    /// </summary>
    public class ScannerProgress
    {
        private int addTotal = 0;
        private int updateTotal = 0;

        private int added = 0;
        private int updated = 0;

        private int addError = 0;
        private int updateError = 0;

        private int removed = 0;
        private int existing = 0;

        public ScannerProgress()
        {
        }
        public ScannerProgress(ScannerProgress item)
        {
            this.addTotal = item.addTotal;
            this.updateTotal = item.updateTotal;

            this.added = item.added;
            this.updated = item.updated;

            this.addError = item.addError;
            this.updateError = item.updateError;

            this.removed = item.removed;
            this.existing = item.existing;
        }

        /// <summary>
        /// The total amount of files to be scanned. <see cref="Total"/> equals <see cref="Error"/> + <see cref="Added"/> + <see cref="Updated"/> when scan has completed.
        /// </summary>
        public int Total
        {
            get { return addTotal + updateTotal; }
        }
        public int Progress
        {
            get { return added + addError + updated + updateError; }
        }

        public int AddTotal
        {
            get { return addTotal; }
            internal set { addTotal = value; }
        }
        /// <summary>
        /// The amount of files to add that were parsed without error.
        /// </summary>
        public int Added
        {
            get { return added; }
            internal set { added = value; }
        }
        public int AddError
        {
            get { return addError; }
            internal set { addError = value; }
        }

        public int UpdateTotal
        {
            get { return updateTotal; }
            internal set { updateTotal = value; }
        }
        /// <summary>
        /// The amount of files to update that were parsed without error.
        /// </summary>
        public int Updated
        {
            get { return updated; }
            internal set { updated = value; }
        }
        public int UpdateError
        {
            get { return updateError; }
            internal set { updateError = value; }
        }

        public int ParseTotal
        {
            get { return addTotal + updateTotal; }
        }
        /// <summary>
        /// The amount of files that were parsed without error. Equals <see cref="Added"/> + <see cref="Updated"/>.
        /// </summary>
        public int Parsed
        {
            get { return added + updated; }
        }

        /// <summary>
        /// The amount of files that failed to parse. Error is less than or equal to <see cref="Total"/>.
        /// </summary>
        public int ParseError
        {
            get { return addError + updateError; }
        }

        /// <summary>
        /// The amount of files that were removed (files that didn't exist).
        /// </summary>
        public int Removed
        {
            get { return removed; }
            internal set { removed = value; }
        }

        /// <summary>
        /// The amount of files registered by the <see cref="AudioScanner"/> when the scan was started.
        /// </summary>
        public int Existing
        {
            get { return existing; }
            internal set { existing = value; }
        }
    }
}
