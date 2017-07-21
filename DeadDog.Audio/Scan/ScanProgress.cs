namespace DeadDog.Audio.Scan
{
    public struct ScanProgress
    {
        public ScannerStates State { get; }

        public int Added { get; }
        public int Updated { get; }
        public int Skipped { get; }
        public int Errors { get; }
        public int Removed { get; }
        public int Total { get; }

        public ScanProgress(ScannerStates state, int added, int updated, int skipped, int errors, int removed, int total)
        {
            State = state;
            Added = added;
            Updated = updated;
            Skipped = skipped;
            Errors = errors;
            Removed = removed;
            Total = total;
        }

        public double Progress => (Added + Updated + Skipped + Errors + Removed) / (double)Total;
    }
}
