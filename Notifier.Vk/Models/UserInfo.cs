using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Notifier.Vk.Converters.Json;

namespace Notifier.Vk.Models
{
    public class UserInfo
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("first_name")]
        public required string FirstName { get; set; }

        [JsonProperty("last_name")]
        public required string LastName { get; set; }

        [JsonProperty("domain")]
        public required string Domain { get; set; }

        [JsonProperty("bdate")]
        [JsonConverter(typeof(VkBirthDateConverter))]
        public BirthDateInfo? BirthDate { get; set; }

        [JsonProperty("city")]
        public CityInfo? City { get; set; }

        [JsonProperty("last_seen")]
        public LastSeenInfo? LastSeen { get; set; }

        [JsonIgnore]
        public required string Url {  get; set; }
    }

    public class BirthDateInfo
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int? Year { get; set; }

        public DateTimeOffset ToDateTimeOffset() => new (Year ?? 1, Month, Day, 0, 0, 0, TimeSpan.Zero);
    }

    public class CityInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }
    }

    public class LastSeenInfo
    {
        [JsonProperty("time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset? Time { get; set; }

        [JsonProperty("platform")]
        public PlatformInfo Platform { get; set; }

        public enum PlatformInfo
        {
            None = 0,
            Mobile = 1,
            IPhone = 2,
            IPad = 3,
            Android = 4,
            WindowsPhone = 5,
            Windows10 = 6,
            Site = 7,
        }
    }
}
