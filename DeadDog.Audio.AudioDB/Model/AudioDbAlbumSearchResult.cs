using Newtonsoft.Json;
using System.Collections.Generic;

namespace DeadDog.Audio.AudioDB.Model
{
    public class AudioDbAlbumSearchResult
    {
        [JsonProperty(PropertyName = "album")]
        public List<AudioDbAlbum> Albums { get; set; }
    }
}
