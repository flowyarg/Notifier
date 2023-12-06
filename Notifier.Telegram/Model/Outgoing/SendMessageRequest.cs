using Newtonsoft.Json;

namespace Notifier.Telegram.Model.Outgoing
{
    public class SendMessageRequest : SendRequest
    {
        [JsonProperty("text")]
        public required string Text { get; set; }

        [JsonProperty("entities")]
        public IReadOnlyCollection<MessageEntity>? Entities { get; set; }

        [JsonProperty("disable_web_page_preview")]
        public bool? DisableWebPagePreview { get; set; }
    }

    public class KeyboardButton
    {
        [JsonProperty("text")]
        public required string Text { get; set; }
    }

    public class ReplyKeyboardMarkup
    {
        [JsonProperty("keyboard")]
        public required IReadOnlyCollection<IReadOnlyCollection<object>> Buttons { get; set; }

        [JsonProperty("is_persistent")]
        public bool? IsPersistent { get; set; }

        [JsonProperty("resize_keyboard")]
        public bool? ResizeKeyboard { get; set; }

        [JsonProperty("one_time_keyboard")]
        public bool? OneTimeKeyboard { get; set; }

        [JsonProperty("input_field_placeholder")]
        public string? InputFieldPlaceholder { get; set; }
    }

    public class ReplyKeyboardRemove
    {
        [JsonProperty("remove_keyboard")]
        public bool RemoveKeyboard { get; set; }
    }

    public class ForceReply
    {
        [JsonProperty("force_reply")]
        public bool Force { get; set; }

        [JsonProperty("input_field_placeholder")]
        public string? InputFieldPlaceholder { get; set; }
    }
}
