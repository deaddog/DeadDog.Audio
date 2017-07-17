using System;
using System.IO;
using System.Text;

namespace DeadDog.Audio
{
    internal static class StreamExtension
    {
        public static string ReadString(this Stream stream)
        {
            return ReadString(stream, Encoding.Unicode);
        }
        public static string ReadString(this Stream stream, Encoding encoding)
        {
            int len = ReadInt32(stream);
            if (len == -1)
                return null;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return encoding.GetString(buffer);
        }
        public static int ReadInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static int? ReadNullableInt32(this Stream stream)
        {
            int isNull = stream.ReadByte();
            if (isNull == 0)
                return ReadInt32(stream);
            else
                return null;
        }

        public static void Write(this Stream stream, string value)
        {
            Write(stream, value, Encoding.Unicode);
        }
        public static void Write(this Stream stream, string value, Encoding encoding)
        {
            if (value == null)
            {
                Write(stream, -1);
                return;
            }
            byte[] buffer = encoding.GetBytes(value);
            Write(stream, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void Write(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }
        public static void Write(this Stream stream, int? value)
        {
            int isNull = value == null ? 1 : 0;
            stream.WriteByte((byte)isNull);
            if (isNull == 0)
                Write(stream, value.Value);
        }
    }
}
