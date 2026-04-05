using System.Text.Json.Serialization;

namespace Notifier.Matrix.Model.Incoming;

//https://playground.matrix.org/#get-/_matrix/client/v3/profile/-userId-
public class ProfileInfoResponse
{
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
    
    [JsonPropertyName("displayname")]
    public required string DisplayName { get; set; }
    
    [JsonPropertyName("m.tz")]
    public string? Timezone { get; set; }
}