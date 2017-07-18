using Flurl.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeadDog.Audio.YouTube
{
    public static class YouTubeMediaParser
    {
        public static async Task<RawTrack> ParseYouTubeTitle(YouTubeID id, string filepath)
        {
            var title = await GetYouTubeTitle(id);
            return ParseYouTubeTitle(title, filepath);
        }
        public static RawTrack ParseYouTubeTitle(string youTubeTitle, string filepath)
        {
            Regex dash = new Regex("^(?<artist>[^-]+)-(?<track>[^-]+)$");
            Regex slash = new Regex("^(?<artist>[^/]+)/(?<track>.+)$");
            Match match;

            string artist = null;
            string album = null;
            string title = youTubeTitle;
            int? tracknumber = null;
            int? year = null;

            if ((match = dash.Match(youTubeTitle)).Success || (match = slash.Match(youTubeTitle)).Success)
            {
                artist = match.Groups["artist"].Value.Trim();
                title = match.Groups["track"].Value.Trim();
            }

            return new RawTrack(filepath, title, album, tracknumber, artist, year);
        }

        public static async Task<string> GetYouTubeTitle(YouTubeID id)
        {
            var html = await $"https://www.youtube.com/watch?v={id.ID}".GetStringAsync();
            var match = Regex.Match(html, "<title>(.*) - YouTube</title>");

            if (match.Success)
                return match.Groups[1].Value;
            else
                return null;
        }
    }
}
