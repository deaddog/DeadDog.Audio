using System.Collections.Generic;

namespace DeadDog.Audio.YouTube
{
    internal static class Sources
    {
        public delegate URL GetYouTubeMp3URL(YouTubeID id);

        public static IEnumerable<GetYouTubeMp3URL> All
        {
            get
            {
                yield return YouTubeInMp3;
            }
        }

        public static URL YouTubeInMp3(YouTubeID id)
        {
            URL mp3infoURL = new URL("http://youtubeinmp3.com/fetch/?api=advanced&video=http://www.youtube.com/watch?v=" + id.ID);
            string html = mp3infoURL.GetHTML();

            int linkIndex = html.IndexOf("<br />Link: ");
            string link = html.Substring(linkIndex + 12);
            link = link.Trim(' ', '\t', '\n', '\r', '\0');

            return new URL(link);
        }
    }
}
