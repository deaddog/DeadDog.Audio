using DeadDog.Audio.AudioDB.Model;
using DeadDog.Audio.Libraries;
using System.Threading.Tasks;

namespace DeadDog.Audio.AudioDB
{
    public static class AudioDbRepositoryExtension
    {
        public static Task<AudioDbAlbumSearchResult> SearchForAlbum(this IAudioDbRepository repository, Album album)
        {
            if (album.HasArtist)
                return repository.SearchForAlbum(album.Title, album.Artist.Name);
            else
                return repository.SearchForAlbum(album.Title);
        }
    }
}
