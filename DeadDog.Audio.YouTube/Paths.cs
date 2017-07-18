using System.IO;

namespace DeadDog.Audio.YouTube
{
    internal static class Paths
    {
        public static string AssemblyDirectoryPath
        {
            get
            {
                var assembly = typeof(Paths).Assembly;
                var location = assembly.Location;

                return Path.GetDirectoryName(location);
            }
        }

        public static string GetExecutablePath(string filename) => Path.Combine(AssemblyDirectoryPath, filename);
    }
}
