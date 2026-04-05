namespace Notifier.DataAccess.Model;

public class MatrixAccessToken : AccessTokenInfo
{
    public required string ClientId { get; set; }
    public required string DeviceId { get; set; }
    public required string DeviceCode { get; set; }
    public required string RefreshToken  { get; set; }
    public required string Scope { get; set; }
    public DateTimeOffset ValidThrough { get; set; }
}