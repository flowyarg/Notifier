using Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Contract;

public interface IMatrixRestClient
{
    Task<string[]> GetApiVersions();

    Task<string> GetUserDisplayName(string userId);
    Task<string[]> GetJoinedRooms();
    Task<LoginFlow[]> GetLegacyLoginFlows();
}