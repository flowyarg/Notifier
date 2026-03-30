using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Outgoing.GetAccessToken;

public class GetAccessTokenRequestViaUsernamePasword
{
    [JsonProperty("identifier")]
    public required UserIdentifier UserIdentifier { get; set; }
    
    [JsonProperty("initial_device_display_name")]
    public required string InitialDeviceDisplayName { get; set; }
    
    [JsonProperty("password")]
    public required string Password { get; set; }
    
    [JsonProperty("type")]
    public string Type => "m.login.password";
}

public class UserIdentifier
{
    [JsonProperty("type")]
    public required string Type { get; set; }
    
    [JsonProperty("user")]
    public required string Username { get; set; }
}
