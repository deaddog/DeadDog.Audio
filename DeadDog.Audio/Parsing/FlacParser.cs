using Luminescence.Xiph;

namespace DeadDog.Audio.Parsing
{
    public class FlacParser : IMediaParser
    {
        public RawTrack ParseTrack(string filepath)
        {
            FlacTagger flac = new FlacTagger(filepath);

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

            return new RawTrack(filepath, flac.Title, flac.Album, trackNumber, flac.Artist, year);
        }
    }
}
