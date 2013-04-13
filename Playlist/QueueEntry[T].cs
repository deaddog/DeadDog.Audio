using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio
{
    public class QueueEntry<T>
    {
        private static int nextid = 0;
        private int id;
        private PlaylistEntry<T> entry;

        public PlaylistEntry<T> Entry
        {
            get { return entry; }
        }

        public QueueEntry(PlaylistEntry<T> entry)
        {
            this.entry = entry;
            this.id = nextid;
            nextid++;
        }

        public int CompareTo(QueueEntry<T> x)
        {
            return this.id.CompareTo(x.id);
        }
    }
}
