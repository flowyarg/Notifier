using Newtonsoft.Json;

namespace Notifier.Telegram.Model.Outgoing
{
    public abstract class SendRequest
    {
        [JsonProperty("chat_id")]
        public long ChatId { get; set; }

        [JsonProperty("parse_mode")]
        public string? ParseMode { get; set; }

        [JsonProperty("disable_notification")]
        public bool? DisableNotification { get; set; }

        [JsonProperty("reply_to_message_id")]
        public int? ReplyToMessageId { get; set; }

        //https://core.telegram.org/bots/api#sendmessage
        [JsonProperty("reply_markup")]
        public object? ReplyMarkup { get; set; }
    }
}
