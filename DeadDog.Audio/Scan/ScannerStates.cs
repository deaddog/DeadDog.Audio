namespace DeadDog.Audio.Scan
{
    /// <summary>
    /// Describes the current state of a media scanner process.
    /// </summary>
    public enum ScannerStates
    {
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
