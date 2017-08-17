using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    public class FrameReader : IDisposable
    {
        internal Stream _stream;
        private List<FrameInfo> _frames = new List<FrameInfo>();

        internal List<Exception> _parsingerrors = new List<Exception>();
        private TagHeader _header;

        public FrameReader(Stream input)
        {
            this._stream = input;
            this._stream.Seek(0, SeekOrigin.Begin);

            this._header = new TagHeader(this);
            if (_header == null || _header.Size <= 0)
            {
                _parsingerrors.Add(new Exception("Unable to parse."));
                return;
            }

            if (_header.Version.Major == 2)
            {
                _parsingerrors.Add(new Exception("ID3 v2.2 not supported"));
            }
            else if (_header.Version.Major > 2)
            {
                while (_stream.Position < _header.FirstFramePosition + _header.Size)
                {
                    BinaryConverter.Skip(0, _stream, _header.FirstFramePosition + (long)_header.Size);
                    if (_stream.Position >= _header.FirstFramePosition + _header.Size)
                        break;

                    FrameInfo frame;
                    try
                    {
                        frame = new FrameInfo(_stream, _header, true);
                    }
                    catch (Exception e)
                    {
                        _parsingerrors.Add(e);
                        frame = null;
                        _stream.Seek(0, SeekOrigin.End);
                    }

                    if (frame != null)
                    {
                        _frames.Add(frame);
                    }
                }
            }
            else
            {
                _parsingerrors.Add(new Exception("Unsupported ID3-version"));
            }

        }

        public T Read<T>(string identifier, Reader<T> reader)
        {
            return Read<T>(identifier, reader, default(T));
        }
        public T Read<T>(string identifier, Reader<T> reader, T notFound)
        {
            FrameInfo frame = findFrame(identifier);
            return Read<T>(frame, reader, notFound);
        }
        private T Read<T>(FrameInfo frame, Reader<T> reader, T notFound)
        {
            if (frame == null)
                return notFound;
            _stream.Seek(frame.Position, System.IO.SeekOrigin.Begin);
            byte[] buffer = new byte[frame.Size];
            _stream.Read(buffer, 0, frame.Size);

            T result;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            using (System.IO.BinaryReader dr = new System.IO.BinaryReader(ms))
            {
                try { result = reader(dr); }
                catch { result = notFound; }
            }
            return result;
        }

        public string ReadString(string identifier) => Read(identifier, ReadString);
        public string ReadString(string identifier, string notFound) => Read(identifier, ReadString, notFound);
        private string ReadString(BinaryReader reader)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding enc = null;
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            if (data.Length <= 1)
                return null;
            string s;
            switch (data[0])
            {
                case 0x00:
                    enc = iso;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                case 0x01:
                    if (data[1] == 0xFF && data[2] == 0xFE)
                        enc = Encoding.Unicode;
                    else if (data[1] == 0xFE && data[2] == 0xFF)
                        enc = Encoding.BigEndianUnicode;
                    else
                        throw new Exception("Unknown Encoding");

                    s = enc.GetString(data, 3, data.Length - 3); ;
                    break;
                case 0x02:
                    enc = Encoding.BigEndianUnicode;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                case 0x03:
                    enc = Encoding.UTF8;
                    s = enc.GetString(data, 1, data.Length - 1);
                    break;
                default:
                    throw new Exception("Unknown Encoding");
            }
            s = s.Trim('\0');
            return s;
        }

        private FrameInfo findFrame(string identifier)
        {
            if (identifier.Length != 4)
                throw new ArgumentException("A frame identifier must be exactly 4 characters long.", "identifier");
            for (int i = 0; i < _frames.Count; i++)
                if (_frames[i].Identifier == identifier)
                    return _frames[i];
            return null;
        }
        public bool Contains(string identifier)
        {
            if (identifier.Length != 4)
                throw new ArgumentException("A frame identifier must be exactly 4 characters long.", "identifier");
            for (int i = 0; i < _frames.Count; i++)
                if (_frames[i].Identifier == identifier)
                    return true;
            return false;
        }
        public string[] GetIdentifiers()
        {
            string[] ids = new string[_frames.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = _frames[i].Identifier;
            return ids;
        }

        public int FrameCount
        {
            get { return _frames.Count; }
        }

        public TagHeader TagHeader
        {
            get { return _header; }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            _stream = null;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public delegate T Reader<T>(BinaryReader reader);
}
