using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

public class GetUserIdResponse
{
    [JsonProperty("device_id")]
    public required string DeviceId { get; set; }
    
    [JsonProperty("user_id")]
    public required string Userid { get; set; }
    
    [JsonProperty("is_guest")]
    public bool IsGuest { get; set; }
}