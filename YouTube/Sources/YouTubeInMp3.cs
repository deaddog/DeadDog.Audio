namespace DeadDog.Audio.YouTube.Sources
{
    internal class YouTubeInMp3 : IYouTubeSource
    {
        public URL GetMp3URL(YouTubeID id)
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
