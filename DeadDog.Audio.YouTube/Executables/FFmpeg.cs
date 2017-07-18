using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DeadDog.Audio.YouTube.Executables
{
    internal static class FFmpeg
    {
        public static async Task ApplyMetaData(RawTrack metadata, string outputPath = null)
        {
            var executablePath = Paths.GetExecutablePath("ffmpeg.exe");

            var setters = new List<string>();
            if (metadata.TrackTitle != null) setters.Add($@"-metadata title=""{metadata.TrackTitle}""");
            if (metadata.AlbumTitle != null) setters.Add($@"-metadata album=""{metadata.AlbumTitle}""");
            if (metadata.ArtistName != null) setters.Add($@"-metadata artist=""{metadata.ArtistName}""");
            if (metadata.TrackNumber != null) setters.Add($@"-metadata track=""{metadata.TrackNumber}""");
            if (metadata.Year != null) setters.Add($@"-metadata year=""{metadata.Year}""");

            var tempPath = Path.GetTempFileName();
            File.Delete(tempPath);
            tempPath = Path.ChangeExtension(tempPath, ".mp3");

            var metadataArgs = "";
            if (setters.Count > 0)
                metadataArgs = string.Join(" ", setters);

            var args = $"-i \"{metadata.Filepath}\" -y -id3v2_version 3 {metadataArgs} -c:a copy \"{tempPath}\"";

            var metaProc = new ProcessStartInfo(executablePath, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            await metaProc.RunProcessAsync();

            File.Copy(tempPath, outputPath ?? metadata.Filepath, true);
            File.Delete(tempPath);
        }
    }
}
