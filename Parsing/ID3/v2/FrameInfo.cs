using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    public class FrameInfo
    {
        private string identifier;
        private long position;
        private int size;
        private FrameStatusFlag statusflags;
        private FrameFormatFlags formatflags;

        public FrameInfo(Stream stream, TagHeader header, bool checkUnsynchronisation)
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, 0, 10);

            this.identifier = Encoding.ASCII.GetString(buffer, 0, 4);
            this.statusflags = (FrameStatusFlag)buffer[8];
            this.formatflags = (FrameFormatFlags)buffer[9];

            int sizeS = BinaryConverter.ToInt32(buffer, 4, true); // sizeS <= sizeU
            int sizeU = BinaryConverter.ToInt32(buffer, 4, false); // sizeU >= sizeS
            this.size = sizeS;

            if (sizeS != sizeU && checkUnsynchronisation)
            {
                int maxsize = header.Size - (int)(stream.Position - header.FirstFramePosition);
                long p = stream.Position;

                if (sizeU == maxsize)
                {
                    stream.Seek(sizeS, SeekOrigin.Current);
                    BinaryConverter.Skip(0, stream, header.FirstFramePosition + (long)header.Size);
                    FrameInfo frameS = new FrameInfo(stream, header, false);
                    stream.Seek(p, SeekOrigin.Begin);

                    if (!char.IsLetterOrDigit(frameS.Identifier, 0) ||
                        !char.IsLetterOrDigit(frameS.Identifier, 1) ||
                        !char.IsLetterOrDigit(frameS.Identifier, 2) ||
                        !char.IsLetterOrDigit(frameS.Identifier, 3) ||
                        frameS.Identifier != frameS.Identifier.ToUpper())
                    {
                        size = sizeU;
                    }
                }
                else
                {
                    stream.Seek(sizeS, SeekOrigin.Current);
                    BinaryConverter.Skip(0, stream, header.FirstFramePosition + (long)header.Size);
                    FrameInfo frameS = new FrameInfo(stream, header, false);
                    stream.Seek(p, SeekOrigin.Begin);

                    stream.Seek(sizeU, SeekOrigin.Current);
                    BinaryConverter.Skip(0, stream, header.FirstFramePosition + (long)header.Size);
                    FrameInfo frameU = new FrameInfo(stream, header, false);
                    stream.Seek(p, SeekOrigin.Begin);

                    if (char.IsLetterOrDigit(frameS.Identifier, 0) &&
                        char.IsLetterOrDigit(frameS.Identifier, 1) &&
                        char.IsLetterOrDigit(frameS.Identifier, 2) &&
                        char.IsLetterOrDigit(frameS.Identifier, 3) &&
                        frameS.Identifier == frameS.Identifier.ToUpper())
                    {
                        size = sizeS;
                    }
                    else if (char.IsLetterOrDigit(frameU.Identifier, 0) &&
                        char.IsLetterOrDigit(frameU.Identifier, 1) &&
                        char.IsLetterOrDigit(frameU.Identifier, 2) &&
                        char.IsLetterOrDigit(frameU.Identifier, 3) &&
                        frameU.Identifier == frameU.Identifier.ToUpper())
                    {
                        size = sizeU;
                    }
                    else
                        throw new Exception("An error occured due to an invalid Unsynchronized integer.");
                }
            }

            this.position = stream.Position;
            stream.Seek(size, SeekOrigin.Current);
        }
        private FrameInfo(string identifier, long position, int size, FrameStatusFlag statusflags, FrameFormatFlags formatflags)
        {
            this.identifier = identifier;
            this.position = position;
            this.size = size;
            this.statusflags = statusflags;
            this.formatflags = formatflags;
        }

        public string Identifier
        {
            get { return identifier; }
        }
        public long Position
        {
            get { return position; }
        }
        public int Size
        {
            get { return size; }
        }

        public byte[] ReadData(System.IO.Stream stream)
        {
            byte[] buffer = new byte[size];
            long p = stream.Position;
            stream.Seek(this.position, System.IO.SeekOrigin.Begin);
            stream.Read(buffer, 0, size);
            stream.Seek(p, System.IO.SeekOrigin.Begin);
            return buffer;
        }

        public override string ToString()
        {
            return identifier + " [size: " + size + " bytes]";
        }
    }
}
