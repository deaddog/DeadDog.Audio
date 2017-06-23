﻿using System.ComponentModel;
using System.IO;

namespace DeadDog.Audio.Libraries
{
    public class Track : INotifyPropertyChanged
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
            internal set
            {
                if (value != _title)
                {
                    _title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
            }
        }
        public int? Tracknumber
        {
            get { return _tracknumber; }
            internal set
            {
                if (value != _tracknumber)
                {
                    _tracknumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tracknumber)));
                }
            }
        }

        public Album Album
        {
            get { return _album; }
            internal set
            {
                if (value != _album)
                {
                    _album = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Album)));
                }
            }
        }
        public Artist Artist
        {
            get { return _artist; }
            internal set
            {
                if (value != _artist)
                {
                    _artist = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Artist)));
                }
            }
        }

        internal Track(RawTrack trackinfo)
        {
            FilePath = trackinfo.Filepath;
            _album = null;
            _artist = null;
            _title = trackinfo.TrackTitle;
            _tracknumber = trackinfo.TrackNumber;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Tracknumber == null ? Title : $"#{Tracknumber} {Title}";
        }
    }
}
