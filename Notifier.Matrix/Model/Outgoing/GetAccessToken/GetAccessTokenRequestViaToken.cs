using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Outgoing.GetAccessToken;

public class GetAccessTokenRequestViaToken
{
    [JsonProperty("token")]
    public required string Token { get; set; }
    
    [JsonProperty("type")] 
    public string Type => "m.login.token";
}