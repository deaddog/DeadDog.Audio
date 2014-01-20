namespace DeadDog.Audio
{
    public class QueueEntry<T>
    {
        private static int nextid = 0;
        private int id;
        private T entry;

        public T Entry
        {
            get { return entry; }
        }

        public QueueEntry(T entry)
        {
            this.entry = entry;
            this.id = nextid;
            nextid++;
        }

        public virtual int CompareTo(QueueEntry<T> x)
        {
            return this.id.CompareTo(x.id);
        }
    }

    public class QueueEntry<T, Q> : QueueEntry<T>
    {
        private Q queueinfo;

        public Q QueueInfo
        {
            get { return queueinfo; }
        }

        public QueueEntry(T entry, Q queueinfo)
            :base(entry)
        {
            this.queueinfo = queueinfo;
        }
    }
}
