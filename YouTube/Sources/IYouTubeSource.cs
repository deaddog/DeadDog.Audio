namespace DeadDog.Audio.YouTube.Sources
{
    internal interface IYouTubeSource
    {
        URL GetMp3URL(YouTubeID id);
    }
}
