using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public abstract class QueueEntry<T, Q>
    {
        private static int nextid = 0;
        private int id;
        private PlaylistEntry<T> entry;
        private Q queueinfo;

        public PlaylistEntry<T> Entry
        {
            get { return entry; }
        }

        public Q QueueInfo
        {
            get { return queueinfo; }
        }

        public QueueEntry(PlaylistEntry<T> entry, Q queueinfo)
        {
            this.entry = entry;
            this.queueinfo = queueinfo;
            this.id = nextid;
            nextid++;
        }

        public abstract int CompareTo(QueueEntry<T, Q> x);

        public int CompareByAddedOrder(QueueEntry<T, Q> x)
        {
            return this.id.CompareTo(x.id);
        }
    }
}
