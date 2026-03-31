using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifier.Matrix.Services;
using Notifier.Matrix.Settings;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using In = Notifier.Matrix.Model.Incoming;


namespace Notifier.Matrix.Implementation;

public class MatrixRestClientWithAuthorization : BaseMatrixRestClient<MatrixRestClientWithNoAuthorization>
{
    private readonly MatrixCredentialsService _credentialsService;
    public MatrixRestClientWithAuthorization(IOptions<MatrixApiSettings> apiSettings,
        IOptions<MatrixApiSettings> matrixApiSettings,
        ILogger<MatrixRestClientWithNoAuthorization> logger, 
        MatrixCredentialsService credentialsService) 
        : base(apiSettings, logger)
    {
        _credentialsService = credentialsService;
    }

    public async Task<string[]> GetJoinedRooms()
    {
        var accessToken = await  _credentialsService.GetAccessToken();
        
        var request = new RestRequest("_matrix/client/v3/joined_rooms")
        {
            Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer")
        };

        var response = await HandleRequest<In.GetJoinedRoomsResponse>(request);
        return response!.Rooms;
    }
    
    public async Task<In.GetUserIdResponse> GetClientId()
    {
        var accessToken = await  _credentialsService.GetAccessToken();

        var request = new RestRequest("_matrix/client/v3/account/whoami")
        {
            Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer")
        };

        var response = await HandleRequest<In.GetUserIdResponse>(request);
        return response!;
    }
}