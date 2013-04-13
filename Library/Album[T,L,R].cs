using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Album<T, L, R> : LibraryItem<R>, IEquatable<string>, IComparer<L>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private bool _isunknown;
        protected bool isunknown
        {
            get { return _isunknown; }
        }

        private string _artistname;
        protected string artistname
        {
            get { return _artistname; }
        }
        private string _title;
        protected string title
        {
            get { return _title; }
        }

        private Track<T, L, R>.TrackCollection tracks;
        public Track<T, L, R>.TrackCollection Tracks
        {
            get { return tracks; }
        }
        private R artist;
        public R Artist
        {
            get { return artist; }
        }

        public Album()
            : base()
        {
        }

        internal override void innerInitialize(RawTrack trackinfo, R owner)
        {
            this._isunknown = trackinfo == RawTrack.Unknown;
            this.tracks = new Track<T, L, R>.TrackCollection(this as L);

            this._title = trackinfo.AlbumTitle;

            this._artistname = trackinfo.ArtistName;
            this.artist = owner;

            if (this._artistname == null)
                this.lowercaseartist = null;
            else
                this.lowercaseartist = this._artistname.ToLower();

            if (this._title == null)
                this.lowercasetitle = null;
            else
                this.lowercasetitle = this._title.ToLower();
        }
        internal override void innerDestroy()
        {
            this._artistname = null;
            this._title = null;
            this.tracks = null;
            this.artist = null;
        }

        public override string ToString()
        {
            return _title;
        }

        public bool Equals(string other)
        {
            return other == _title;
        }
        public int Compare(L x, L y)
        {
            return x._title.CompareTo(y._title);
        }

        private string lowercasetitle;
        private string lowercaseartist;
        internal bool SearchForStrings(SearchMethods method, List<string> strings)
        {
            if (strings.Count == 0)
                return true;
            else if (strings.Count == 1)
            {
                if (Contains(strings[0]))
                {
                    strings.RemoveAt(0);
                    return true;
                }
                else
                    return false;
            }
            else if (method == SearchMethods.ContainsAll)
            {
                for (int i = 0; i < strings.Count; i++)
                    if (Contains(strings[i]))
                    {
                        strings.RemoveAt(i);
                        i--;
                    }
                return (strings.Count == 0);
            }
            else // method == SearchMethod.ContainsAny
            {
                for (int i = 0; i < strings.Count; i++)
                    if (Contains(strings[i]))
                    {
                        strings.RemoveAt(i);
                        return true;
                    }
                return false;
            }
        }
        internal bool Contains(string text)
        {
            return Contains(lowercasetitle, text, false)
                || Contains(lowercaseartist, text, false);
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
