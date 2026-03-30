using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifier.Matrix.Model.Outgoing;
using Notifier.Matrix.Model.Outgoing.GetAccessToken;
using Notifier.Matrix.Settings;
using RestSharp;
using In = Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Implementation;

public class MadgeburgMatrixRestClientWithNoAuthorization : BaseMatrixRestClient<MadgeburgMatrixRestClientWithNoAuthorization>
{
    public MadgeburgMatrixRestClientWithNoAuthorization(IOptions<MatrixApiSettings> apiSettings,
        ILogger<MadgeburgMatrixRestClientWithNoAuthorization> logger) : base(apiSettings, logger, baseUrl: "https://chat.magdeburg.jetzt")
    {
    }
    
    //https://chat.magdeburg.jetzt/auth/oauth2/token
    public async Task<In.GetAccessTokenResponse> RefreshAccessToken(string clientId, string refreshToken)
    {
        var request = new RestRequest("auth/oauth2/token")
        {
            Method = Method.Post,
        };
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("refresh_token", refreshToken);
        request.AddParameter("client_id", clientId);
        
        var response = await HandleRequest<In.GetAccessTokenResponse>(request);
        return response!;
    }
    
    //https://chat.magdeburg.jetzt/auth/oauth2/token
    public async Task<In.GetAccessTokenResponse> GetAccessToken(string clientId, string deviceCode)
    {
        var request = new RestRequest("auth/oauth2/token")
        {
            Method = Method.Post,
        };
        request.AddParameter("client_id", clientId);
        request.AddParameter("device_code", deviceCode);
        request.AddParameter("grant_type", "urn:ietf:params:oauth:grant-type:device_code");
        
        var response = await HandleRequest<In.GetAccessTokenResponse>(request);
        return response!;
    }
    
    public async Task<In.AuthorizeDeviceResponse> AuthorizeDevice(string clientId, string deviceId)
    {
        var request = new RestRequest("auth/oauth2/device")
        {
            Method = Method.Post
        };
        request.AddParameter("client_id", clientId);
        request.AddParameter("scope", $"urn:matrix:client:api:* urn:matrix:client:device:{deviceId}");

        var response = await HandleRequest<In.AuthorizeDeviceResponse>(request);
        return response!;
    }
    
    public async Task<In.RegisterClientResponse> RegisterClient()
    {
        var request = new RestRequest("auth/oauth2/registration")
        {
            Method = Method.Post
        };
        request.AddJsonBody(new
        {
            client_name = "MyBotApp",
            client_uri = "https://example.com/",
            logo_uri = "https://example.com/logo.png",
            tos_uri = "https://example.com/tos.html",
            policy_uri = "https://example.com/policy.html",
            redirect_uris = new[]
            {
                "http://localhost/callback",
            },
            token_endpoint_auth_method = "none",
            response_types = new[] { "code" },
            grant_types = new[]
            {
                "authorization_code",
                "refresh_token",
                "urn:ietf:params:oauth:grant-type:token-exchange",
                "urn:ietf:params:oauth:grant-type:device_code"
            },
            application_type = "native"
        });

        var response = await HandleRequest<In.RegisterClientResponse>(request);
        return response!;
    }
}