using Notifier.Matrix.Contract;
using Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Implementation;

public class MatrixRestClient : IMatrixRestClient
{
    private readonly MatrixRestClientWithNoAuthorization _clientWithNoAuthorization;
    private readonly MatrixRestClientWithAuthorization _clientWithAuthorization;

    public MatrixRestClient(MatrixRestClientWithNoAuthorization clientWithNoAuthorization, MatrixRestClientWithAuthorization clientWithAuthorization)
    {
        _clientWithNoAuthorization = clientWithNoAuthorization;
        _clientWithAuthorization = clientWithAuthorization;
    }

    public Task<string[]> GetApiVersions() => _clientWithNoAuthorization.GetApiVersions();
    public Task<string> GetUserDisplayName(string userId) => _clientWithNoAuthorization.GetUserDisplayName(userId);
    public Task<string[]> GetJoinedRooms() => _clientWithAuthorization.GetJoinedRooms();

    public Task<LoginFlow[]> GetLegacyLoginFlows() => _clientWithNoAuthorization.GetLegacyLoginFlows();
}