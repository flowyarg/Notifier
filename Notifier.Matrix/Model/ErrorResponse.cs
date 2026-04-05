using Newtonsoft.Json;

namespace Notifier.Matrix.Model;

internal class ErrorResponse
{
    [JsonProperty("errcode")]
    public string ErrorCode { get; set; }
    
    [JsonProperty("error")]
    public string ErrorMessage { get; set; }
}