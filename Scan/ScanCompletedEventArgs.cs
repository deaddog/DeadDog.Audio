using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio.Scan
{
    public class ScanCompletedEventArgs : EventArgs
    {
        private string[] removedFiles;
        private string[] errorFiles;

        private RawTrack[] addedData;
        private RawTrack[] updatedData;

        private RawTrack[] existingData;

        public ScanCompletedEventArgs(IEnumerable<System.IO.FileInfo> removedFiles,
            IEnumerable<System.IO.FileInfo> errorFiles,
            IEnumerable<RawTrack> addedFiles,
            IEnumerable<RawTrack> updatedFiles,
            IEnumerable<RawTrack> existingFiles)
            : this(removedFiles.Select(x => x.FullName), errorFiles.Select(x => x.FullName), addedFiles, updatedFiles, existingFiles)
        {
        }
        public ScanCompletedEventArgs(IEnumerable<string> removedFiles,
            IEnumerable<string> errorFiles,
            IEnumerable<RawTrack> addedFiles,
            IEnumerable<RawTrack> updatedFiles,
            IEnumerable<RawTrack> existingFiles)
        {
            this.removedFiles = removedFiles.ToArray();
            this.errorFiles = errorFiles.ToArray();
            this.addedData = addedFiles.ToArray();
            this.updatedData = updatedFiles.ToArray();
            this.existingData = existingFiles.ToArray();
        }

        public string[] RemovedFiles
        {
            get { return removedFiles; }
        }
        public string[] ErrorFiles
        {
            get { return errorFiles; }
        }
        public RawTrack[] AddedData
        {
            get { return addedData; }
        }
        public RawTrack[] UpdatedData
        {
            get { return updatedData; }
        }
        public RawTrack[] ExistingData
        {
            get { return existingData; }
        }
    }
}
