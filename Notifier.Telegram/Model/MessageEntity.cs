using Newtonsoft.Json;
using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Model
{
    public class MessageEntity
    {
        [JsonProperty("type")]
        public MessageEntityType Type { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("custom_emoji_id")]
        public string? CustomEmojiId {  get; set; }
    }
}
