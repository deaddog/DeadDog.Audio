using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Track<T, L, R> : LibraryItem<L>, IEquatable<string>, IComparer<T>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private System.IO.FileInfo _file;
        public bool FileExist
        {
            get { _file.Refresh(); return _file.Exists; }
        }
        public string FilePath
        {
            get { return _file.FullName; }
        }

        private string _title;
        protected string title
        {
            get { return _title; }
        }

        private bool _hasnumber;
        protected bool hasnumber
        {
            get { return _hasnumber; }
        }
        private int _tracknumber;
        protected int tracknumber
        {
            get { return _tracknumber; }
        }

        private L album;
        public L Album
        {
            get { return album; }
        }

        private string _albumtitle;
        protected string albumtitle
        {
            get { return _albumtitle; }
        }

        private string _artistname;
        protected string artistname
        {
            get { return _artistname; }
        }

        public Track()
            : base()
        {

        }

        internal override void innerInitialize(RawTrack trackinfo, L owner)
        {
            this._file = trackinfo.File;

            this.album = owner;
            this._albumtitle = trackinfo.AlbumTitle;

            this._artistname = trackinfo.ArtistName;

            this._title = trackinfo.TrackTitle;
            this._hasnumber = !trackinfo.TrackNumberUnknown;
            this._tracknumber = trackinfo.TrackNumber;

            if (this._artistname == null)
                this.lowercaseartist = null;
            else
                this.lowercaseartist = this._artistname.ToLower();

            if (this._albumtitle == null)
                this.lowercasealbum = null;
            else
                this.lowercasealbum = this._albumtitle.ToLower();

            if (this._title == null)
                this.lowercasetitle = null;
            else
                this.lowercasetitle = this._title.ToLower();
        }
        internal override void innerDestroy()
        {
            this._albumtitle = null;
            this._artistname = null;
            this.album = null;
            this._file = null;
            this._title = null;
            this._hasnumber = false;
            this._tracknumber = RawTrack.TrackNumberIfUnknown;
        }

        internal int tracknumberCompare(Track<T, L, R> other)
        {
            if (!this.hasnumber && !other.hasnumber)
                return 0;
            else if (!this.hasnumber)
                return -1;
            else if (!other.hasnumber)
                return 1;
            else
                return this.tracknumber.CompareTo(other.tracknumber);
        }
        internal int tracktitleCompare(Track<T, L, R> other)
        {
            if (this.title == null && other.title == null)
                return 0;
            else if (this.title == null)
                return -1;
            else if (other.title == null)
                return 1;
            else
                return this.title.CompareTo(other.title);
        }

        public override string ToString()
        {
            return _title;
        }

        public bool Equals(string other)
        {
            return other == _title;
        }
        public virtual int Compare(T x, T y)
        {
            int number = x.tracknumberCompare(y);
            if (number == 0)
                return x.tracktitleCompare(y);
            else
                return number;
        }

        private string lowercasetitle;
        private string lowercasealbum;
        private string lowercaseartist;
        internal bool SearchForStrings(SearchMethods method, string[] strings)
        {
            if (strings.Length == 0)
                return true;
            else if (strings.Length == 1)
                return Contains(strings[0]);
            else if (method == SearchMethods.ContainsAll)
            {
                for (int i = 0; i < strings.Length; i++)
                    if (!Contains(strings[i]))
                        return false;
                return true;
            }
            else // method == SearchMethod.ContainsAny
            {
                for (int i = 0; i < strings.Length; i++)
                    if (Contains(strings[i]))
                        return true;
                return false;
            }
        }
        internal bool Contains(string text)
        {
            return Contains(lowercasetitle, text, true) 
                || Contains(lowercaseartist, text, false) 
                || Contains(lowercasealbum, text, false);
        }
        private bool Contains(string instr, string text, bool nullval)
        {
            if (instr == null)
                return nullval;
            else
                return instr.Contains(text);
        }
    }
}
