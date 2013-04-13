using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Library
{
    public abstract partial class Artist<T, L, R> : LibraryItem<Library<T, L, R>>, IEquatable<string>, IComparer<R>
        where T : Track<T, L, R>
        where L : Album<T, L, R>
        where R : Artist<T, L, R>
    {
        private bool _isunknown;
        protected bool isunknown
        {
            get { return _isunknown; }
        }

        private string _name;
        protected string name
        {
            get { return _name; }
        }

        private Album<T, L, R>.AlbumCollection albums;
        public Album<T, L, R>.AlbumCollection Albums
        {
            get { return albums; }
        }
        private Library<T, L, R> library;
        public Library<T, L, R> Library
        {
            get { return library; }
        }

        public Artist()
            : base()
        {
        }

        internal override void innerInitialize(RawTrack trackinfo, Library<T, L, R> owner)
        {
            L unknownAlbum = owner.Factory.CreateAlbum();
            unknownAlbum.initialize(RawTrack.Unknown, this as R);

            this.albums = new Album<T,L,R>.AlbumCollection(this as R, unknownAlbum);
            this._isunknown = trackinfo == RawTrack.Unknown;

            this._name = trackinfo.ArtistName;
            this.library = owner;

            if (this._name == null)
                this.lowercasename = null;
            else
                this.lowercasename = this._name.ToLower();
        }
        internal override void innerDestroy()
        {
            this._name = null;

            albums.UnknownAlbum.destroy();
            this.albums = null;

            this.library = null;
        }

        public override string ToString()
        {
            return _name;
        }

        public bool Equals(string other)
        {
            return other == _name;
        }
        public int Compare(R x, R y)
        {
            return x._name.CompareTo(y._name);
        }

        private string lowercasename;
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
            return Contains(lowercasename, text, false);
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
