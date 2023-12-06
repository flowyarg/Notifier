using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Notifier.Telegram.Model.Incoming
{
    //https://core.telegram.org/bots/api#update
    public class Update
    {
        [JsonProperty("update_id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery? CallbackQuery { get; set; }
    }

    //https://core.telegram.org/bots/api#message
    public class Message
    {
        [JsonProperty("message_id")]
        public int Id { get; set; }

        [JsonProperty("from")]
        public User? From { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("via_bot")]
        public User? ViaBot { get; set; }

        [JsonProperty("edit_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset? EditDate { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("entities")]
        public IReadOnlyCollection<MessageEntity>? MessageEntities { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public required string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }
    }

    public class Chat
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public required string Type { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageEntityType
    {
        [EnumMember(Value = "mention")]
        Mention,
        [EnumMember(Value = "hashtag")]
        HashTag,
        [EnumMember(Value = "cashtag")]
        CashTag,
        [EnumMember(Value = "bot_command")]
        BotCommand,
        [EnumMember(Value = "url")]
        Url,
        [EnumMember(Value = "email")]
        Email,
        [EnumMember(Value = "phone_number")]
        PhoneNumber,
        [EnumMember(Value = "text_link")]
        TextLink,
        [EnumMember(Value = "bold")]
        BoldText,
        [EnumMember(Value = "italic")]
        ItalicText,
        [EnumMember(Value = "underline")]
        UnderlinedText,
        [EnumMember(Value = "strikethrough")]
        StrikethroughText,
        [EnumMember(Value = "spoiler")]
        Spoiler,
        [EnumMember(Value = "custom_emoji")]
        CustomEmoji
    }

    //https://core.telegram.org/bots/api#callbackquery
    public class CallbackQuery
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("from")]
        public required User From { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }

        [JsonProperty("chat_instance")]
        public required string ChatInstance { get; set; }

        [JsonProperty("data")]
        public string? Data { get; set; }
    }
}
