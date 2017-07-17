using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    public class FrameReader : IDisposable
    {
        internal System.IO.Stream stream;
        private List<FrameInfo> frames = new List<FrameInfo>();

        internal List<Exception> parsingerrors = new List<Exception>();
        private TagHeader header;

        public FrameReader(System.IO.Stream input)
        {
            this.stream = input;
            this.stream.Seek(0, SeekOrigin.Begin);

            this.header = new TagHeader(this);
            if (header == null || header.Size <= 0)
            {
                parsingerrors.Add(new Exception("Unable to parse."));
                return;
            }

            if (header.Version.Major == 2)
            {
                parsingerrors.Add(new Exception("ID3 v2.2 not supported"));
            }
            else if (header.Version.Major > 2)
            {
                while (stream.Position < header.FirstFramePosition + header.Size)
                {
                    BinaryConverter.Skip(0, stream, header.FirstFramePosition + (long)header.Size);
                    if (stream.Position >= header.FirstFramePosition + header.Size)
                        break;

                    FrameInfo frame;
                    try
                    {
                        frame = new FrameInfo(stream, header, true);
                    }
                    catch (Exception e)
                    {
                        parsingerrors.Add(e);
                        frame = null;
                        stream.Seek(0, SeekOrigin.End);
                    }

                    if (frame != null)
                    {
                        frames.Add(frame);
                    }
                }
            }
            else
            {
                parsingerrors.Add(new Exception("Unsupported ID3-version"));
            }

        }
        
        public T Read<T>(string identifier, Reader<T> reader)
        {
            return Read<T>(identifier, reader, default(T));
        }
        public T Read<T>(string identifier, Reader<T> reader, T notfound)
        {
            FrameInfo frame = findFrame(identifier);
            return Read<T>(frame, reader, notfound);
        }
        private T Read<T>(FrameInfo frame, Reader<T> reader, T notfound)
        {
            if (frame == null)
                return notfound;
            stream.Seek(frame.Position, System.IO.SeekOrigin.Begin);
            byte[] buffer = new byte[frame.Size];
            stream.Read(buffer, 0, frame.Size);

            T result;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            using (System.IO.BinaryReader dr = new System.IO.BinaryReader(ms))
            {
                try { result = reader(dr); }
                catch { result = notfound; }
            }
            return result;
        }

        private FrameInfo findFrame(string identifier)
        {
            if (identifier.Length != 4)
                throw new ArgumentException("A frame identifier must be exactly 4 characters long.", "identifier");
            for (int i = 0; i < frames.Count; i++)
                if (frames[i].Identifier == identifier)
                    return frames[i];
            return null;
        }
        public bool Contains(string identifier)
        {
            if (identifier.Length != 4)
                throw new ArgumentException("A frame identifier must be exactly 4 characters long.", "identifier");
            for (int i = 0; i < frames.Count; i++)
                if (frames[i].Identifier == identifier)
                    return true;
            return false;
        }
        public string[] GetIdentifiers()
        {
            string[] ids = new string[frames.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = frames[i].Identifier;
            return ids;
        }

        public int FrameCount
        {
            get { return frames.Count; }
        }

        public TagHeader TagHeader
        {
            get { return header; }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            stream = null;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public delegate T Reader<T>(System.IO.BinaryReader reader);
}
