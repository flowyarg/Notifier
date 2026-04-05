namespace Notifier.DataAccess.Model;

public class VkVideoAccessToken : AccessTokenInfo
{
    public required string ApiVersion  { get; set; }
    public required string ClientId { get; set; }
    public required string Scopes { get; set; }
}