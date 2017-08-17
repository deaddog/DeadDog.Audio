using Luminescence.Xiph;

namespace DeadDog.Audio.Parsing
{
    public class OggParser : IMediaParser
    {
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            OggTagger ogg = new OggTagger(filepath);

            int? trackNumber;
            if (int.TryParse(ogg.TrackNumber, out int t))
                trackNumber = t;
            else
                trackNumber = null;

            int? year;
            if (int.TryParse(ogg.Date, out int y))
                year = y;
            else
                year = null;

            track = new RawTrack(filepath, ogg.Title, ogg.Album, trackNumber, ogg.Artist, year);
            return true;
        }
    }
}
