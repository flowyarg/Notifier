using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

//https://spec.matrix.org/latest/client-server-api/#api-versions
public class GetApiVersionsResponse
{
    [JsonProperty("unstable_features")]
    public Dictionary<string, string>? UnstableFeatures { get; set; }
    
    [JsonProperty("versions")]
    public required string[] Versions { get; set; }
}