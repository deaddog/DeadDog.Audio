using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Scan
{
    public class ScannerTracer
    {
        private ScannerProgress progress = new ScannerProgress();
        private ScannerState state = ScannerState.NotRunning;

        public ScannerTracer()
            : this(new ScannerProgress(), ScannerState.NotRunning)
        {
        }
        private ScannerTracer(ScannerProgress progress, ScannerState state)
        {
            this.progress = progress;
            this.state = state;
        }

        public ScannerProgress Progress
        {
            get { return progress; }
            internal set { progress = value; }
        }
        public ScannerState State
        {
            get { return state; }
            internal set { state = value; }
        }
    }
}
