using Newtonsoft.Json;

namespace Notifier.Telegram.Model.Outgoing
{
    public class SendPhotoRequest : SendRequest
    {
        [JsonProperty("photo")]
        public required string PhotoUrl { get; set; }

        [JsonProperty("caption")]
        public string? Caption { get; set; }

        [JsonProperty("caption_entities")]
        public IReadOnlyCollection<MessageEntity>? CaptionEntities { get; set; }

        [JsonProperty("has_spoiler")]
        public bool? HasSpoiler { get; set; }
    }
}
