using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class VideoAlbumInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonIgnore]
        public string Url { get; set; }
    }
}
