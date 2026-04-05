using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

public class AuthorizeDeviceResponse
{
    [JsonProperty("device_code")]
    public required string DeviceCode { get; set; }
    
    [JsonProperty("user_code")]
    public required string UserCode { get; set; }
    
    [JsonProperty("verification_uri")]
    public required string VerificationUri { get; set; }
    
    [JsonProperty("verification_uri_complete")]
    public string? VerificationUriComplete { get; set; }
    
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonProperty("interval")]
    public int Interval { get; set; }
}