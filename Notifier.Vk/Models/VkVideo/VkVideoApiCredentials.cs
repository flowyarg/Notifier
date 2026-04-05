namespace Notifier.Vk.Models.VkVideo;

public class VkVideoApiCredentials
{
    public required string ApiVersion { get; set; }
    public required string ClientId { get; set; }
    public required string AccessToken { get; set; }
    public required string Scopes { get; set; }
}