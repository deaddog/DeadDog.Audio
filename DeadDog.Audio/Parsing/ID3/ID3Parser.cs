namespace DeadDog.Audio.Parsing.ID3
{
    public class ID3Parser : IMediaParser
    {
        private readonly ID3v1Parser _v1Parser;
        private readonly ID3v2Parser _v2Parser;

        public ID3Parser()
        {
            _v1Parser = new ID3v1Parser();
            _v2Parser = new ID3v2Parser();
        }

        public bool TryParseTrack(string filepath, out RawTrack track)
        {
            if (_v2Parser.TryParseTrack(filepath, out track))
                return true;
            else
                return _v1Parser.TryParseTrack(filepath, out track);
        }
    }
}
