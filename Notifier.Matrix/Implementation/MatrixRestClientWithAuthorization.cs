using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifier.Matrix.Settings;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using In = Notifier.Matrix.Model.Incoming;


namespace Notifier.Matrix.Implementation;

public class MatrixRestClientWithAuthorization : BaseMatrixRestClient<MatrixRestClientWithNoAuthorization>
{
    public MatrixRestClientWithAuthorization(IOptions<MatrixApiSettings> apiSettings,
        IOptions<MatrixApiSettings> matrixApiSettings,
        ILogger<MatrixRestClientWithNoAuthorization> logger) 
        : base(apiSettings, logger,
        authenticator: new OAuth2AuthorizationRequestHeaderAuthenticator(matrixApiSettings.Value.AccessToken, "Bearer"))
    {
    }

    public async Task<string[]> GetJoinedRooms()
    {
        var request = new RestRequest("_matrix/client/v3/joined_rooms");

        var response = await HandleRequest<In.GetJoinedRoomsResponse>(request);
        return response!.Rooms;
    }
    
    public async Task<In.GetUserIdResponse> GetClientId()
    {
        var request = new RestRequest("_matrix/client/v3/account/whoami");

        var response = await HandleRequest<In.GetUserIdResponse>(request);
        return response!;
    }
}