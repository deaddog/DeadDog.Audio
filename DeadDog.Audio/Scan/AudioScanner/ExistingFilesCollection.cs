using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner
    {
        public class ExistingFilesCollection : IEnumerable<RawTrack>
        {
            private List<RawTrack> files;
            private Comparer comparer;

            public ExistingFilesCollection()
            {
                this.files = new List<RawTrack>();
                this.comparer = new Comparer();
            }

            public void Add(RawTrack item)
            {
                int index = files.BinarySearch(item, comparer);
                if (~index < 0)
                    files.Add(item);
                else
                    files.Insert(~index, item);
            }
            public void AddRange(IEnumerable<RawTrack> collection)
            {
                foreach (RawTrack rt in collection)
                    Add(rt);
            }

            public bool Remove(string filepath)
            {
                if (filepath == null)
                    return false;

                int index = files.BinarySearch(RawTrack.Unknown, new CompareToString(filepath));
                if (index >= 0)
                {
                    files.RemoveAt(index);
                    return true;
                }
                else
                    return false;
            }
            public bool Remove(RawTrack item)
            {
                return files.Remove(item);
            }

            public void Clear()
            {
                files.Clear();
            }

            public bool Contains(string filepath)
            {
                if (filepath == null)
                    return false;

                int index = files.BinarySearch(RawTrack.Unknown, new CompareToString(filepath));
                return index >= 0;
            }
            public bool Contains(RawTrack item)
            {
                return files.Contains(item);
            }

            public int Count
            {
                get { return files.Count; }
            }

            public void CopyTo(RawTrack[] array, int arrayIndex)
            {
                files.CopyTo(array, arrayIndex);
            }
            public RawTrack[] ToArray()
            {
                return files.ToArray();
            }

            private class CompareToString : IComparer<RawTrack>
            {
                private System.IO.FileInfo file;

                public CompareToString(string path)
                {
                    this.file = new System.IO.FileInfo(path);
                }

                public int Compare(RawTrack x, RawTrack y)
                {
                    return x.Filepath.CompareTo(file.FullName);
                }
            }


            private class Comparer : IComparer<RawTrack>
            {
                public int Compare(RawTrack x, RawTrack y)
                {
                    return x.Filepath.CompareTo(y.Filepath);
                }
            }

            public IEnumerator<RawTrack> GetEnumerator()
            {
                return files.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return files.GetEnumerator();
            }
        }
    }
}
