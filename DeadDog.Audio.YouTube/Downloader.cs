using System.Threading.Tasks;

namespace DeadDog.Audio.YouTube
{
    public static class Downloader
    {
        public static async Task<RawTrack> Download(this YouTubeID id, string destinationFilepath)
        {
            var meta = await YouTubeMediaParser.ParseYouTubeTitle(id, destinationFilepath);

            await Executables.YoutubeDL.Download(id, destinationFilepath);
            await Executables.FFmpeg.ApplyMetaData(meta);

            return meta;
        }
    }
}
