namespace DeadDog.Audio.Libraries
{
    public class TrackCollection : LibraryCollectionBase<Track>
    {
        internal TrackCollection() : base(CompareTracks)
        {
        }

        internal override Track _unknownElement
        {
            get { return null; }
        }

        public event TrackEventHandler TrackAdded, TrackRemoved;
        protected override void OnAdded(Track element)
        {
            TrackEventArgs e = new TrackEventArgs(element);
            if (TrackAdded != null)
                TrackAdded(this, e);
        }
        protected override void OnRemoved(Track element)
        {
            TrackEventArgs e = new TrackEventArgs(element);
            if (TrackRemoved != null)
                TrackRemoved(this, e);
        }

        private static int CompareTracks(Track element1, Track element2)
        {
            int? v1 = element1.Tracknumber, v2 = element2.Tracknumber;
            if (v1.HasValue)
                return v2.HasValue ? v1.Value.CompareTo(v2.Value) : 1;
            else
                return v2.HasValue ? -1 : 0;
        }
    }
}
