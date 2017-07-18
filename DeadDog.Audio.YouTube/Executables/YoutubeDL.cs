using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DeadDog.Audio.YouTube.Executables
{
    internal static class YoutubeDL
    {
        public static async Task Download(YouTubeID id, string destinationPath)
        {
            var executablePath = Paths.GetExecutablePath("youtube-dl.exe");

            string downloadArgs = $"https://www.youtube.com/watch?v={id.ID} -x --audio-format mp3 --audio-quality 0 -o \"%(id)s.%(ext)s\"";
            ProcessStartInfo downloadProc = new ProcessStartInfo(executablePath, downloadArgs)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            await downloadProc.RunProcessAsync();

            var filepath = Paths.GetExecutablePath($"{id.ID}.mp3");
            File.Copy(filepath, destinationPath, true);
            File.Delete(filepath);
        }
    }
}
