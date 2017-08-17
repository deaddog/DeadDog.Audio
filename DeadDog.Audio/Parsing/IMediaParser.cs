namespace DeadDog.Audio.Parsing
{
    /// <summary>
    /// Provides generic methods for parsing metadata from files to <see cref="RawTrack"/> items.
    /// </summary>
    public interface IMediaParser
    {
        /// <summary>
        /// Reads metadata from an audio-file.
        /// </summary>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <param name="track">When the method returns, contains the read metadata, if parsing succeeded, or null if parsing failed. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the file was parsed successfully; otherwise, <c>false</c>.</returns>
        bool TryParseTrack(string filepath, out RawTrack track);
    }

    public static class DataParserExtension
    {
        /// <summary>
        /// Reads metadata from an audio-file.
        /// </summary>
        /// <param name="parser">The <see cref="IMediaParser"/> used for parsing metadata.</param>
        /// <param name="filepath">The full path of the file from which to read.</param>
        /// <param name="track">
        /// When the method returns, contains the read metadata, if parsing succeeded, or null if parsing failed.
        /// Parsing fails if any exception is thrown from the <see cref="IMediaParser.TryParseTrack(string, out RawTrack)"/> method.
        /// This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the file was parsed successfully; otherwise, <c>false</c>.</returns>
        public static bool SafeTryParseTrack(this IMediaParser parser, string filepath, out RawTrack track)
        {
            try
            {
                return parser.TryParseTrack(filepath, out track);
            }
            catch
            {
                track = null;
                return false;
            }
        }
    }
}
