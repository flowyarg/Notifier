namespace Notifier.Vk.Models
{
    using Converters.Json;
    using Newtonsoft.Json;

    public class GroupInfo
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(VkVideoIntToStringFieldConverter))]
        public required string Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("screen_name")]
        public required string ScreenName { get; set; }

        [JsonIgnore]
        public required string Url { get; set; }
    }
}
