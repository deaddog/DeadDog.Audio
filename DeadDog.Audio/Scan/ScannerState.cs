﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Scan
{
    /// <summary>
    /// Describes the current state of an <see cref="AudioScanner"/>.
    /// </summary>
    public enum ScannerState
    {
        /// <summary>
        /// The scanner is not running and has not been started.
        /// </summary>
        NotStarted,
        /// <summary>
        /// The scanner has been started and is retrieving a collection of files to parse.
        /// </summary>
        Scanning,
        /// <summary>
        /// The scanner has been started and is parsing data from files.
        /// </summary>
        Parsing,
        /// <summary>
        /// The scanner has completed.
        /// </summary>
        Completed
    }
}
