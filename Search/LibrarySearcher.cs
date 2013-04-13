using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class LibrarySearcher
    {
        private Library library;

        public LibrarySearcher(Library library)
        {
            this.library = library;
        }

        public IEnumerable<Track> Search(SearchMethods method, string searchstring)
        {
            return Search(method, searchstring.Trim().Split(' '));
        }
        public IEnumerable<Track> Search(SearchMethods method, params string[] searchstring)
        {
            throw new NotImplementedException();
        }
    }
}
