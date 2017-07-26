using DeadDog.Audio.AudioDB.Model;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace DeadDog.Audio.AudioDB
{
    public class AudioDbRepository : IAudioDbRepository
    {
        private const int AUDIO_DB_API_KEY_TEST = 1;

        private readonly int _apiKey;

        public AudioDbRepository(int apiKey = AUDIO_DB_API_KEY_TEST)
        {
            _apiKey = apiKey;
        }

        public async Task<AudioDbAlbumSearchResult> SearchForAlbum(string albumTitle)
        {
            albumTitle = albumTitle?.Trim() ?? throw new ArgumentNullException(nameof(albumTitle));

            return await $"http://www.theaudiodb.com/api/v1/json/{_apiKey}/searchalbum.php?s=Various%20Artists&a={albumTitle}".GetJsonAsync<AudioDbAlbumSearchResult>();
        }
        public async Task<AudioDbAlbumSearchResult> SearchForAlbum(string albumTitle, string artistName)
        {
            artistName = artistName?.Trim() ?? throw new ArgumentNullException(nameof(artistName));
            albumTitle = albumTitle?.Trim() ?? throw new ArgumentNullException(nameof(albumTitle));

            return await $"http://www.theaudiodb.com/api/v1/json/{_apiKey}/searchalbum.php?s={artistName}&a={albumTitle}".GetJsonAsync<AudioDbAlbumSearchResult>();
        }

        public async Task<byte[]> DownloadFile(Uri url)
        {
            return await url.AbsoluteUri.GetBytesAsync();
        }
    }
}
