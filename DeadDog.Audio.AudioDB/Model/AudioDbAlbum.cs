using Newtonsoft.Json;
using System;

namespace DeadDog.Audio.AudioDB.Model
{
    public class AudioDbAlbum
    {
        [JsonProperty(PropertyName = "idAlbum")]
        public string AlbumId { get; set; }
        [JsonProperty(PropertyName = "idArtist")]
        public string ArtistId { get; set; }
        [JsonProperty(PropertyName = "idLabel")]
        public string LabelId { get; set; }

        [JsonProperty(PropertyName = "strAlbum")]
        public string AlbumName { get; set; }
        [JsonProperty(PropertyName = "strAlbumStripped")]
        public string AlbumNameStripped { get; set; }
        [JsonProperty(PropertyName = "strArtist")]
        public string ArtistName { get; set; }
        [JsonProperty(PropertyName = "strArtistStripped")]
        public string ArtistNameStripped { get; set; }
        [JsonProperty(PropertyName = "strLabel")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "intYearReleased")]
        public int? ReleaseYear { get; set; }
        [JsonProperty(PropertyName = "strStyle")]
        public string Style { get; set; }
        [JsonProperty(PropertyName = "strGenre")]
        public string Genre { get; set; }
        [JsonProperty(PropertyName = "strReleaseFormat")]
        public string ReleaseFormat { get; set; }

        [JsonProperty(PropertyName = "intSales")]
        public int? Sales { get; set; }
        [JsonProperty(PropertyName = "strAlbumThumb")]
        public Uri AlbumThumbnail { get; set; }
        [JsonProperty(PropertyName = "strAlbumThumbBack")]
        public Uri AlbumThumbnailBack { get; set; }
        [JsonProperty(PropertyName = "strAlbumCDart")]
        public Uri AlbumCDArt { get; set; }
        [JsonProperty(PropertyName = "strAlbumSpine")]
        public Uri AlbumSpine { get; set; }

        [JsonProperty(PropertyName = "strDescriptionEN")]
        public string DescriptionEN { get; set; }
        [JsonProperty(PropertyName = "strDescriptionDE")]
        public string DescriptionDE { get; set; }
        [JsonProperty(PropertyName = "strDescriptionFR")]
        public string DescriptionFR { get; set; }
        [JsonProperty(PropertyName = "strDescriptionCN")]
        public string DescriptionCN { get; set; }
        [JsonProperty(PropertyName = "strDescriptionIT")]
        public string DescriptionIT { get; set; }
        [JsonProperty(PropertyName = "strDescriptionJP")]
        public string DescriptionJP { get; set; }
        [JsonProperty(PropertyName = "strDescriptionRU")]
        public string DescriptionRU { get; set; }
        [JsonProperty(PropertyName = "strDescriptionES")]
        public string DescriptionES { get; set; }
        [JsonProperty(PropertyName = "strDescriptionPT")]
        public string DescriptionPT { get; set; }
        [JsonProperty(PropertyName = "strDescriptionSE")]
        public string DescriptionSE { get; set; }
        [JsonProperty(PropertyName = "strDescriptionNL")]
        public string DescriptionNL { get; set; }
        [JsonProperty(PropertyName = "strDescriptionHU")]
        public string DescriptionHU { get; set; }
        [JsonProperty(PropertyName = "strDescriptionNO")]
        public string DescriptionNO { get; set; }
        [JsonProperty(PropertyName = "strDescriptionIL")]
        public string DescriptionIL { get; set; }
        [JsonProperty(PropertyName = "strDescriptionPL")]
        public string DescriptionPL { get; set; }

        [JsonProperty(PropertyName = "intLoved")]
        public int? Loved { get; set; }
        [JsonProperty(PropertyName = "intScore")]
        public int? Score { get; set; }
        [JsonProperty(PropertyName = "intScoreVotes")]
        public int? ScoreVotes { get; set; }
        
        [JsonProperty(PropertyName = "strMood")]
        public string Mood { get; set; }
        [JsonProperty(PropertyName = "strTheme")]
        public string Theme { get; set; }
        [JsonProperty(PropertyName = "strSpeed")]
        public string Speed { get; set; }
        [JsonProperty(PropertyName = "strMusicBrainzID")]
        public Guid MusicBrainzID { get; set; }
        [JsonProperty(PropertyName = "strMusicBrainzArtistID")]
        public Guid MusicBrainzArtistID { get; set; }
    }
}
