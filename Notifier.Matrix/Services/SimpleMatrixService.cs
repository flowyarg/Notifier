using System.Diagnostics;
using Newtonsoft.Json;
using Notifier.Matrix.Contract;
using Notifier.Matrix.Implementation;

namespace Notifier.Matrix.Services;

public class SimpleMatrixService
{
    private readonly IMatrixRestClient _client;
    private readonly MadgeburgMatrixRestClientWithNoAuthorization _authClient;
    private readonly MatrixRestClientWithAuthorization _authedClient;

    public SimpleMatrixService(IMatrixRestClient client, MadgeburgMatrixRestClientWithNoAuthorization authClient, MatrixRestClientWithAuthorization authedClient)
    {
        _client = client;
        _authClient = authClient;
        _authedClient = authedClient;
    }


    private async Task Register()
    {
        var deviceId = "blablabla";
        var clientId = "blablabla";
        var deviceCode = "blablabla";
        
        // var registerResponse = await _authClient.RegisterClient();
        // Debugger.Break();
        // return;
        
        try
        {
            // var deviceAuthorizationResponse = await _authClient.AuthorizeDevice(clientId, deviceId);
            // var verificationUri = deviceAuthorizationResponse.VerificationUriComplete
            //     ?? $"{deviceAuthorizationResponse.VerificationUri}?user_code={deviceAuthorizationResponse.UserCode}";
            // Debug.WriteLine($"Go to url: {verificationUri}");

            while (true)
            {
                try
                {
                    var tokenResponse = await _authClient.GetAccessToken(clientId, deviceCode);
                    await File.WriteAllTextAsync("accessToken.json", JsonConvert.SerializeObject(tokenResponse));
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
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
    
    private async Task RefreshTokenTest()
    {
        var clientId = "blablabla";
        var refreshToken = "blablabla";
        try
        {
            var test = await _authClient.RefreshAccessToken(clientId, refreshToken);
            Debugger.Break();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
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