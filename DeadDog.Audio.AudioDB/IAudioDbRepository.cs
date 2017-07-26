using System;
using System.Threading.Tasks;
using DeadDog.Audio.AudioDB.Model;

namespace DeadDog.Audio.AudioDB
{
    public interface IAudioDbRepository
    {
        Task<byte[]> DownloadFile(Uri url);
        Task<AudioDbAlbumSearchResult> SearchForAlbum(string albumTitle);
        Task<AudioDbAlbumSearchResult> SearchForAlbum(string albumTitle, string artistName);
    }
}