using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

public class RegisterClientResponse
{
    [JsonProperty("client_id")]
    public required string ClientId { get; set; }

    [JsonProperty("client_name")]
    public required string ClientName { get; set; }
    
    [JsonProperty("client_uri")]
    public required string ClientUri { get; set; }
    
    [JsonProperty("logo_uri")]
    public required string LogoUri { get; set; }
    
    [JsonProperty("tos_uri")]
    public required string TosUri { get; set; }
    
    [JsonProperty("policy_uri")]
    public required string PolicyUri { get; set; }
    
    [JsonProperty("redirect_uris")]
    public required string[] RedirectUris { get; set; }
    
    [JsonProperty("token_endpoint_auth_method")]
    public required string TokenEndpointAuthMethod { get; set; }
    
    [JsonProperty("response_types")]
    public required string[] ResponseTypes { get; set; }
    
    [JsonProperty("grant_types")]
    public required string[] GrantTypes { get; set; }
    
    [JsonProperty("application_type")]
    public required string ApplicationType { get; set; }
}