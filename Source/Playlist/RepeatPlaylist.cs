namespace DeadDog.Audio.Playlist
{
    public class RepeatPlaylist<T> : DecoratorPlaylist<T>
    {
        public RepeatPlaylist(IPlaylist<T> playlist, bool repeat) : base(playlist)
        {
            Repeat = repeat;
        }

        public bool Repeat { get; set; }

        public override bool MoveNext()
        {
            if (base.MoveNext())
                return true;
            else if (Repeat)
                return base.MoveToFirst();
            else
                return false;
        }
        public override bool MovePrevious()
        {
            if (base.MovePrevious())
                return true;
            else if (Repeat)
                return base.MoveToLast();
            else
                return false;
        }
    }
}
