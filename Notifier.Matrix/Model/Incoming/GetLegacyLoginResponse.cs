using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

public class GetLegacyLoginResponse
{
    [JsonProperty("flows")]
    public required LoginFlow[] LoginFlows { get; set; }
}

public class LoginFlow
{
    [JsonProperty("type")]
    public required string Type { get; set; }
}