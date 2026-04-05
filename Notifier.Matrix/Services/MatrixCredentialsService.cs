using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifier.DataAccess;
using Notifier.DataAccess.Model;
using Notifier.Matrix.Implementation;
using Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Services;

public class MatrixCredentialsService
{
    private class AccessToken
    {
        public required string ClientId { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public DateTimeOffset ValidThrough { get; set; }
    }
    
    private const int AccessTokenId = 2;
    
    private readonly MadgeburgMatrixRestClientWithNoAuthorization _authClient;
    private readonly IDbContextFactory<NotifierDbContext> _dbContextFactory;
    private readonly ILogger<MatrixCredentialsService> _logger;
    
    private AccessToken? _accessToken;

    public MatrixCredentialsService(MadgeburgMatrixRestClientWithNoAuthorization authClient, IDbContextFactory<NotifierDbContext> dbContextFactory, ILogger<MatrixCredentialsService> logger)
    {
        _authClient = authClient;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    private async Task RegisterClient()
    {
        var registerResponse = await _authClient.RegisterClient();
        Debugger.Break();
    }
    
    private async Task RegisterDevice(string clientId, string deviceId)
    {
        var deviceAuthorizationResponse = await _authClient.AuthorizeDevice(clientId, deviceId);
        var verificationUri = deviceAuthorizationResponse.VerificationUriComplete
            ?? $"{deviceAuthorizationResponse.VerificationUri}?user_code={deviceAuthorizationResponse.UserCode}";
        Debug.WriteLine($"Go to url: {verificationUri}");
        
        while (true)
        {
            try
            {
                var requestStartTime = DateTimeOffset.UtcNow;
                var tokenResponse = await _authClient.GetAccessToken(clientId, deviceAuthorizationResponse.DeviceCode);
                
                await StoreNewAccessToken(deviceId, clientId, deviceAuthorizationResponse.DeviceCode, tokenResponse, requestStartTime);
                Debugger.Break();
                break;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"No token for now: {e.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5 + 1));
            }
        }
    }

    public async Task<string> GetAccessToken()
    {
        _accessToken ??= await LoadAccessToken();
        if (_accessToken is not null && _accessToken.ValidThrough > DateTimeOffset.UtcNow)
            return _accessToken.Token;
        
        _logger.LogInformation("Refreshing access token...");
        var requestStartTime = DateTimeOffset.UtcNow;
        var tokenResponse = await _authClient.RefreshAccessToken(_accessToken!.ClientId, _accessToken.RefreshToken);
        await StoreRefreshedAccessToken(tokenResponse,  requestStartTime);
        return _accessToken.Token;
    }

    private async Task<AccessToken> LoadAccessToken()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var accessToken = await dbContext.MatrixAccessTokens.FindAsync(AccessTokenId);
        return accessToken is null
            ? throw new Exception("No token found")
            : new AccessToken
            {
                ClientId =  accessToken.ClientId,
                Token = accessToken.AccessToken,
                RefreshToken = accessToken.RefreshToken,
                ValidThrough = accessToken.ValidThrough
            };
    }

    private async Task StoreRefreshedAccessToken(GetAccessTokenResponse response, DateTimeOffset requestStartTime)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var accessToken = await dbContext.MatrixAccessTokens.FindAsync(AccessTokenId);
        if (accessToken is null)
            throw new Exception("No token found");
        accessToken.AccessToken = response.AccessToken;
        accessToken.RefreshToken = response.RefreshToken;
        accessToken.ValidThrough = requestStartTime.AddMinutes(response.ExpiresIn);
        accessToken.Scope = response.Scope;
        await dbContext.SaveChangesAsync();
    }

    private async Task StoreNewAccessToken(string deviceId, string clientId, string deviceCode, GetAccessTokenResponse response, DateTimeOffset requestStartTime)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var accessToken = await dbContext.MatrixAccessTokens.FindAsync(AccessTokenId);
        if (accessToken is null)
        {
            _ = dbContext.MatrixAccessTokens.Add(new MatrixAccessToken
            {
                Id = AccessTokenId,
                AccessToken = response.AccessToken,
                RefreshToken =  response.RefreshToken,
                ValidThrough = requestStartTime.AddMinutes(response.ExpiresIn),
                DeviceId = deviceId,
                ClientId =  clientId,
                DeviceCode = deviceCode,
                Scope = response.Scope
            });
        }
        else
        {
            accessToken.AccessToken = response.AccessToken;
            accessToken.RefreshToken = response.RefreshToken;
            accessToken.ValidThrough = requestStartTime.AddMinutes(response.ExpiresIn);
            accessToken.DeviceId = deviceId;
            accessToken.ClientId = clientId;
            accessToken.DeviceCode = deviceCode;
            accessToken.Scope = response.Scope;
        }

        await dbContext.SaveChangesAsync();
    }
}