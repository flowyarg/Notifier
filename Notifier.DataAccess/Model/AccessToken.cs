namespace Notifier.DataAccess.Model;

public abstract class AccessTokenInfo
{
    public int Id { get; set; }
    
    public required string AccessToken { get; set; }
}