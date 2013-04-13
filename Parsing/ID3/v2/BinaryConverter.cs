using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.ID3
{
    public static class BinaryConverter
    {
        public static int ToInt32(byte[] buffer, int index, bool synchsafe)
        {
            int shift = 8;
            if (synchsafe)
                shift = 7;
            else
            { }
            int i = buffer[index];
            i <<= shift;
            i |= buffer[index + 1];
            i <<= shift;
            i |= buffer[index + 2];
            i <<= shift;
            i |= buffer[index + 3];

            return i;
        }


        public static void Skip(int skipval, System.IO.Stream instream, long positionlessthan)
        {
            while (instream.ReadByte() == 0 && instream.Position < positionlessthan)
            {
            }
            if (instream.Position < positionlessthan)
                instream.Seek(-1, System.IO.SeekOrigin.Current);
        }
    }
}
