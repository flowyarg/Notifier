namespace Notifier.Vk.Models
{
    using Converters.Json;
    using Newtonsoft.Json;

    public class VideoAlbumInfo
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(VkVideoIntToStringFieldConverter))]
        public string Id { get; set; }

        [JsonProperty("owner_id")]
        [JsonConverter(typeof(VkVideoIntToStringFieldConverter))]
        public string OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonIgnore]
        public string Url { get; set; }
    }
}
