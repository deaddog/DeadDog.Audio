namespace DeadDog.Audio.Parsing.ID3
{
    public class ID3Parser : IMediaParser
    {
        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            var v1Info = ID3v1.FromFile(filepath);
            var v2Info = new ID3v2(filepath);

            if (!v1Info.TagFound && !v2Info.TagFound)
            {
                track = null;
                return false;
            }

            var artist = Longest(v2Info.Artist, v1Info.Artist);
            var album = Longest(v2Info.Album, v1Info.Album);
            var title = Longest(v2Info.Title, v1Info.Title);

            var tracknumber = GetTrackNumber(v1Info, v2Info);
            var year = GetYear(v1Info, v2Info);

            track = new RawTrack(filepath, title, album, tracknumber, artist, year);
            return true;
        }

        private string Longest(string a, string b)
        {
            if (a == null)
                return b;
            if (b == null)
                return a;

            if (a.Length >= b.Length)
                return a;
            return b;
        }

        private int? GetTrackNumber(ID3v1 v1, ID3v2 v2)
        {
            if (v2.TrackNumber > 0)
                return v2.TrackNumber;
            else if (v1.TrackNumber > 0)
                return v1.TrackNumber;
            else
                return null;
        }
        private int? GetYear(ID3v1 v1, ID3v2 v2)
        {
            if (v2.Year > 0)
                return v2.Year;
            else
                return v1.Year;
        }
    }
}
