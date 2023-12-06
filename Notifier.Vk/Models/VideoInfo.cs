using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Notifier.Vk.Models
{
    public class VideoInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("adding_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset PublicationDate { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("duration")]
        public int DurationSeconds { get; set; }

        [JsonProperty("image")]
        public IReadOnlyCollection<VideoPreviewInfo>? VideoPreviews { get; set; }

        [JsonProperty("player")]
        public string Url { get; set; }
    }

    public class VideoPreviewInfo
    {
        [JsonProperty("url")]
        public string ImageUrl { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
