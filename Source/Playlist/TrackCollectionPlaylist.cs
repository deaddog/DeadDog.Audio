using DeadDog.Audio.Libraries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DeadDog.Audio.Playlist
{
    public class TrackCollectionPlaylist : IPlaylist<Track>
    {
        private readonly LibraryCollection<Track> _collection;
        private readonly Playlist<Track> _playlist;

        public TrackCollectionPlaylist(LibraryCollection<Track> collection)
        {
            _collection = collection;
            _playlist = new Playlist<Track>();

            _playlist.EntryChanged += (s, e) => EntryChanged?.Invoke(this, e);
            _playlist.EntryChanging += (s, e) => EntryChanging?.Invoke(this, e);

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

        public Track Entry
        {
            get { return _playlist.Entry; }
        }

        public event EventHandler EntryChanged;
        public event EntryChangingEventHandler<Track> EntryChanging;

        public void Reset()
        {
            _playlist.Reset();
        }

        public bool MoveNext()
        {
            return _playlist.MoveNext();
        }
        public bool MovePrevious()
        {
            return _playlist.MovePrevious();
        }

        public bool MoveToFirst()
        {
            return _playlist.MoveToFirst();
        }
        public bool MoveToLast()
        {
            return _playlist.MoveToLast();
        }

        public bool MoveToEntry(Track entry)
        {
            return _playlist.MoveToEntry(entry);
        }
        public bool Contains(Track entry)
        {
            return _playlist.Contains(entry);
        }

        IEnumerator<Track> IEnumerable<Track>.GetEnumerator()
        {
            return (_playlist as IEnumerable<Track>).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_playlist as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
