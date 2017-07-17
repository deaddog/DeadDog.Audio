using DeadDog.Audio.Libraries;
using System;
using System.Collections.Specialized;

namespace DeadDog.Audio.Playlist
{
    public class TrackCollectionPlaylist : DecoratorPlaylist<Track>
    {
        private readonly SortedPlaylist<Track> _sortedPlaylist;
        private readonly Playlist<Track> _playlist;
        private readonly LibraryCollection<Track> _collection;

        public TrackCollectionPlaylist(LibraryCollection<Track> collection, Comparison<Track> comparison = null)
            : this(collection, comparison, comparison == null ? null : new SortedPlaylist<Track>(comparison), comparison == null ? new Playlist<Track>() : null)
        {
        }
        private TrackCollectionPlaylist(LibraryCollection<Track> collection, Comparison<Track> comparison, SortedPlaylist<Track> sortedPlaylist, Playlist<Track> playlist) : base((IPlaylist<Track>)sortedPlaylist ?? playlist)
        {
            _sortedPlaylist = sortedPlaylist;
            _playlist = playlist;
            _collection = collection;

            if (_playlist != null)
            {
                foreach (var track in collection)
                    _playlist.Add(track);

                _collection.CollectionChanged += TracksCollectionChanged;
            }
            else if (_sortedPlaylist != null)
            {
                foreach (var track in collection)
                    _sortedPlaylist.Add(track);

                _collection.CollectionChanged += SortedTracksCollectionChanged;
            }
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
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException("The playlist does not support more than one track moving at a time.");
                    else
                        _playlist.MoveEntry((Track)e.OldItems[0], e.NewStartingIndex);
                    break;

                default:
                    throw new NotSupportedException($"The collection change action {e.Action} is not supported by the playlist.");
            }
        }

        private void SortedTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        _sortedPlaylist.Add((Track)e.NewItems[i]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        _playlist.Remove((Track)e.OldItems[i]);
                    break;

                case NotifyCollectionChangedAction.Move:
                    // Move events are ignored for the sorted playlist
                    break;

                default:
                    throw new NotSupportedException($"The collection change action {e.Action} is not supported by the playlist.");
            }
        }
    }
}
