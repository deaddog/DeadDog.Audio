using System.IO;

namespace DeadDog.Audio.Libraries
{
    public class Track
    {
        private string _title;
        private int? _tracknumber;

        private Album _album;
        private Artist _artist;

        public bool FileExist => File.Exists(FilePath);
        public string FilePath { get; }

        public string Title
        {
            get { return _title; }
            internal set { _title = value; }
        }
        public int? Tracknumber
        {
            get { return _tracknumber; }
            internal set { _tracknumber = value; }
        }

        public Album Album
        {
            get { return _album; }
            internal set { _album = value; }
        }
        public Artist Artist
        {
            get { return _artist; }
            internal set { _artist = value; }
        }

        internal Track(RawTrack trackinfo)
        {
            FilePath = trackinfo.Filepath;
            _album = null;
            _artist = null;
            _title = trackinfo.TrackTitle;
            _tracknumber = trackinfo.TrackNumber;
        }

        public override string ToString()
        {
            return Tracknumber == null ? Title : $"#{Tracknumber} {Title}";
        }
    }
}
