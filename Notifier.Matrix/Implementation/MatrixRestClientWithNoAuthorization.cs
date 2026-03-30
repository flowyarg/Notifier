using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifier.Matrix.Settings;
using RestSharp;
using In = Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Implementation;

public class MatrixRestClientWithNoAuthorization : BaseMatrixRestClient<MatrixRestClientWithNoAuthorization>
{
    public MatrixRestClientWithNoAuthorization(IOptions<MatrixApiSettings> apiSettings,
        ILogger<MatrixRestClientWithNoAuthorization> logger) : base(apiSettings, logger)
    {
    }

    public async Task<string[]> GetApiVersions()
    {
        var request = new RestRequest("_matrix/client/versions");

        var response = await HandleRequest<In.GetApiVersionsResponse>(request);
        return response!.Versions;
    }
    
    public async Task<string> GetUserDisplayName(string userId)
    {
        var request = new RestRequest("_matrix/client/v3/profile");
        request.Parameters.AddParameter(new QueryParameter("userId", userId));

        var response = await HandleRequest<In.ProfileInfoResponse>(request);
        return response!.DisplayName;
    }
    
    public async Task<In.LoginFlow[]> GetLegacyLoginFlows()
    {
        var request = new RestRequest("_matrix/client/v3/login");

        var response = await HandleRequest<In.GetLegacyLoginResponse>(request);
        return response!.LoginFlows;
    }
}