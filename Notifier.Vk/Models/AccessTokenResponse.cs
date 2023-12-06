using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public required string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("user_id")]
        public required string UserId { get; set; }
    }
}
