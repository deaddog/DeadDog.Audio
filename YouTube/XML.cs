using System;
using System.IO;

namespace DeadDog.Audio.YouTube
{
    internal static class XML
    {
        public static string DocumentPath(string directory)
        {
            return Path.Combine(Path.GetFullPath(directory), "youtube.xml");
        }
    }
}
