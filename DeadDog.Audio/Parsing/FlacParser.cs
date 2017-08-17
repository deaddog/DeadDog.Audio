using Luminescence.Xiph;

namespace DeadDog.Audio.Parsing
{
    public class FlacParser : IMediaParser
    {
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            var flac = new FlacTagger(filepath);

            int? trackNumber;
            if (int.TryParse(flac.TrackNumber, out int t))
                trackNumber = t;
            else
                trackNumber = null;

            int? year;
            if (int.TryParse(flac.Date, out int y))
                year = y;
            else
                year = null;

            track = new RawTrack(filepath, flac.Title, flac.Album, trackNumber, flac.Artist, year);
            return true;
        }
    }
}
