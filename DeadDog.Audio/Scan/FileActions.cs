namespace DeadDog.Audio.Scan
{
    /// <summary>
    /// Describes the state of a file after it has been parsed.
    /// </summary>
    public enum FileActions
    {
        /// <summary>
        /// The file was successfully parsed and is new.
        /// </summary>
        Added = 0x01,
        /// <summary>
        /// The file was successfully parsed and has previously been parsed.
        /// </summary>
        Updated = 0x02,
        /// <summary>
        /// The file could not be parsed. Either the <see cref="FileActions.AddError"/> or the <see cref="FileActions.UpdateError"/> should also be set when this value is set.
        /// </summary>
        Error = 0x04,
        /// <summary>
        /// The file could not be parsed and is new.
        /// </summary>
        AddError = 0x0C,
        /// <summary>
        /// The file could not be parsed and has previously been parsed.
        /// </summary>
        UpdateError = 0x14,
        /// <summary>
        /// The file can no longer be found.
        /// </summary>
        Removed = 0x20,
        /// <summary>
        /// The file was skipped.
        /// </summary>
        Skipped = 0x40
    }
}
