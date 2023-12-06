using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class GroupInfo
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("screen_name")]
        public required string ScreenName { get; set; }

        [JsonIgnore]
        public required string Url { get; set; }
    }
}
