using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Audio.Parsing.ID3
{
    public class TagHeader
    {
        private static TagHeader empty;
        static TagHeader()
        {
            empty = new TagHeader(new Version(), (TagFlags)0, 0, 0);
            empty.isempty = true;
        }

        public static TagHeader Empty
        {
            get { return empty; }
        }
        public static bool IsEmpty(TagHeader item)
        {
            return item.isempty;
        }

        private bool isempty = false;
        private Version version;
        private TagFlags flags;
        private int size;
        private long firstframe;

        public TagHeader(FrameReader reader)
        {
            byte[] buffer = new byte[9];
            bool found = false;
            int offset = 0;
            this.version = new Version(0, 0);
            while (reader._stream.Position < reader._stream.Length && !found && reader._stream.Position < 200)
            {
                int b = reader._stream.ReadByte();
                if (b == 0x49)
                {
                    reader._stream.Read(buffer, offset, 9);
                    string text = Encoding.ASCII.GetString(buffer);
                    if (buffer[0] == 0x44 &&
                        buffer[1] == 0x33 &&
                        buffer[2] < 0xff &&
                        buffer[3] < 0xff &&
                        //buffer[4] is the flag byte
                        buffer[5] < 0x80 &&
                        buffer[6] < 0x80 &&
                        buffer[7] < 0x80 &&
                        buffer[8] < 0x80)
                    {
                        this.version = new Version(buffer[2], buffer[3]);
                        this.flags = (TagFlags)buffer[4];
                        this.size = BinaryConverter.ToInt32(buffer, 5, true);

                        if (this.size + reader._stream.Position > reader._stream.Length)
                        {
                            this.version = new Version(0, 0);
                            break;
                        }

                        if ((flags & TagFlags.ExtendedHeader) == TagFlags.ExtendedHeader)
                        {
                            reader._stream.Read(buffer, 0, 4);
                            int extendedsize = BinaryConverter.ToInt32(buffer, 0, true);
                            reader._stream.Seek(extendedsize - 4, SeekOrigin.Current);
                        }

                        this.firstframe = reader._stream.Position;
                        break;
                    }
                    else
                        reader._stream.Seek(-9, SeekOrigin.Current);
                }
            }
            if (version == new Version(0, 0))
            {
                this.isempty = true;
                this.version = empty.version;
                this.flags = empty.flags;
                this.size = empty.size;
                this.firstframe = empty.firstframe;
            }
        }
        public TagHeader(Version version, TagFlags flags, int size, long firstframe)
        {
            this.version = version;
            this.flags = flags;
            this.size = size;
            this.firstframe = firstframe;
        }

        public static bool operator ==(TagHeader left, TagHeader right)
        {
            if (Object.ReferenceEquals(left, null) && Object.ReferenceEquals(right, null))
                return true;
            if (Object.ReferenceEquals(left, null))
                return false;
            if (Object.ReferenceEquals(right, null))
                return false;
            if (left.isempty && right.isempty)
                return true;
            else
                return Object.ReferenceEquals(left, right);
        }
        public static bool operator !=(TagHeader left, TagHeader right)
        {
            if (left.isempty && right.isempty)
                return false;
            else
                return !Object.ReferenceEquals(left, right);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TagHeader))
                return false;
            TagHeader o = obj as TagHeader;
            return o.isempty == this.isempty &&
                   o.version == this.version &&
                   o.flags == this.flags &&
                   o.size == this.size &&
                   o.firstframe == this.firstframe;
        }
        public override int GetHashCode()
        {
            if (isempty)
                return -1;
            return version.GetHashCode();
        }

        public Version Version
        {
            get { return version; }
        }
        public TagFlags Flags
        {
            get { return flags; }
        }
        public int Size
        {
            get { return size; }
        }
        public long FirstFramePosition
        {
            get { return firstframe; }
        }
    }
}
