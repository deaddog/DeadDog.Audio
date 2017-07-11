using DeadDog.Audio.Libraries;
using System;
using System.Collections.Specialized;

namespace DeadDog.Audio.Playlist
{
    public class TrackCollectionPlaylist : DecoratorPlaylist<Track>
    {
        private readonly Playlist<Track> _playlist;
        private readonly LibraryCollection<Track> _collection;

        public TrackCollectionPlaylist(LibraryCollection<Track> collection) : this(collection, new Playlist<Track>())
        {
        }
        private TrackCollectionPlaylist(LibraryCollection<Track> collection, Playlist<Track> playlist) : base(playlist)
        {
            _playlist = playlist;
            _collection = collection;

            foreach (var track in collection)
                _playlist.Add(track);

            _collection.CollectionChanged += TracksCollectionChanged;
        }

        private void TracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        _playlist.Insert(e.NewStartingIndex + i, (Track)e.NewItems[i]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        _playlist.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    var temp = _playlist[e.OldStartingIndex];
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException("The playlist does not support more than one track moving at a time.");
                    _playlist.RemoveAt(e.OldStartingIndex);
                    _playlist.Insert(e.NewStartingIndex, (Track)e.OldItems[0]);
                    break;

                default:
                    throw new NotSupportedException($"The collection change action {e.Action} is not supported by the playlist.");
            }
        }
    }
}
