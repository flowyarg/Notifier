using System.Diagnostics;
using Notifier.Matrix.Implementation;

namespace Notifier.Matrix.Services;

public class SimpleMatrixService
{
    private readonly MatrixRestClientWithAuthorization _authedClient;

    public SimpleMatrixService(MatrixRestClientWithAuthorization authedClient)
    {
        _authedClient = authedClient;
    }
    
    private async Task GetRoomsTest()
    {
        try
        {
            var test = await _authedClient.GetJoinedRooms();
            Debugger.Break();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    public Task Test()
        => GetRoomsTest();
}