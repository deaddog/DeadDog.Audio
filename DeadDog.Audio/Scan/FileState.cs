using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Scan
{
    /// <summary>
    /// Describes the state of a file after it has been parsed using an <see cref="AudioScanner"/>.
    /// </summary>
    public enum FileState
    {
        /// <summary>
        /// The file was successfully parsed and is new to the <see cref="AudioScanner"/>.
        /// </summary>
        Added = 0x01,
        /// <summary>
        /// The file was successfully parsed and has previously been parsed by the <see cref="AudioScanner"/>.
        /// </summary>
        Updated = 0x02,
        /// <summary>
        /// The file could not be parsed. Either the <see cref="FileState.AddError"/> or the <see cref="FileState.UpdateError"/> should also be set when this value is set.
        /// </summary>
        Error = 0x04,
        /// <summary>
        /// The file could not be parsed and is new to the <see cref="AudioScanner"/>.
        /// </summary>
        AddError = 0x0C,
        /// <summary>
        /// The file could not be parsed and has previously been parsed by the <see cref="AudioScanner"/>.
        /// </summary>
        UpdateError = 0x14,
        /// <summary>
        /// The file can no longer be found by the <see cref="AudioScanner"/>.
        /// </summary>
        Removed = 0x20,
        /// <summary>
        /// The file is skipped.
        /// </summary>
        Skipped = 0x40
    }
}
