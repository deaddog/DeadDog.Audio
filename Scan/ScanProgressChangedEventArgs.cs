using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio.Scan
{
    public class ScanProgressChangedEventArgs : EventArgs
    {
        private ScannerProgress progress = new ScannerProgress();
        private ScannerState state = ScannerState.NotRunning;

        internal ScanProgressChangedEventArgs(ScannerTracer tracer)
            : this(tracer.Progress, tracer.State)
        {
        }
        public ScanProgressChangedEventArgs(ScannerProgress progress, ScannerState state)
        {
            this.progress = new ScannerProgress(progress);
            this.state = state;
        }

        public ScannerProgress Progress
        {
            get { return progress; }
        }
        public ScannerState State
        {
            get { return state; }
        }
    }
}
