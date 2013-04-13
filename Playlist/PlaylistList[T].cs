using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    public class PlaylistList<T> : PlaylistCollection<T>, IList<IPlaylist<T>>
    {
        #region IList<IPlaylist<T>> Members

        public int IndexOf(IPlaylist<T> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IPlaylist<T> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IPlaylist<T> this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<IPlaylist<T>> Members

        public void Add(IPlaylist<T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IPlaylist<T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IPlaylist<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(IPlaylist<T> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IPlaylist<T>> Members

        public IEnumerator<IPlaylist<T>> GetEnumerator()
        {
            foreach (var pl in getPlaylists())
                yield return pl;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var pl in getPlaylists())
                yield return pl;
        }

        #endregion
    }
}
