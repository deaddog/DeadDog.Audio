﻿using DeadDog.Audio.Libraries;

namespace DeadDog.Audio.Playlist
{
    public class ArtistPlaylist : TrackCollectionPlaylist
    {
        public ArtistPlaylist(Artist artist) : base(artist.Tracks)
        {
            Artist = artist;
        }

        public Artist Artist { get; }
    }
}
