using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadDog.Audio.Libraries;

namespace DeadDog.Audio
{
    class ArtistPlaylist<T> : IPlaylist<T>
    {
        List<AlbumPlaylist<T>> albums;
        int index;
        Artist artist;

        public ArtistPlaylist(Artist artist)
        {
            this.artist = artist;
            this.artist.Albums.a
        }

        public PlaylistEntry<T> CurrentEntry
        {
            get { return albums[index].CurrentEntry; }
        }

        public bool MoveNext()
        {
            if (index == -2)
            {
                return false;
            }
            else if (index == -1)
            {
                index++;
                while (albums[index].MoveNext() == false)
                {
                    index++;
                    if (index >= albums.Count)
                    {
                        index = -2;
                        return false;
                    }
                }
                return true;
            }
            else if (index < albums.Count)
            {
                if (albums[index].MoveNext())
                    return true;
                else
                {
                    index++;
                    albums[index].MoveNext();
                    return true;
                }
            }
            else return false;
        }

        public bool MovePrevious()
        {
            if (index == -2)
                return false;
            else if(index == -1)
        }

        public bool MoveRandom()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
