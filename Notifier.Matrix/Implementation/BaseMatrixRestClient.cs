using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Notifier.Matrix.Contract;
using Notifier.Matrix.Model;
using Notifier.Matrix.Settings;
using RestSharp;
using RestSharp.Authenticators;
using In = Notifier.Matrix.Model.Incoming;

namespace Notifier.Matrix.Implementation;

public abstract class BaseMatrixRestClient<TChild> : RestClient
{
    private readonly JsonSerializerSettings _serializationSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    protected readonly MatrixApiSettings _apiSettings;
    protected readonly ILogger<TChild> _logger;
    
    //https://magdeburg.jetzt/_matrix/client/v3/
    protected BaseMatrixRestClient(IOptions<MatrixApiSettings> apiSettings, ILogger<TChild> logger, string? baseUrl = null, IAuthenticator? authenticator = null)
        : base(new RestClientOptions(baseUrl ?? "https://magdeburg.jetzt")
        {
            Authenticator = authenticator
        })
    {
        _apiSettings = apiSettings.Value;
        _logger = logger;
    }

  
    
    // public async Task<bool> SendMessage<T>(T message)
    //     where T : SendRequest
    // {
    //     var body = JsonConvert.SerializeObject(message, _serializationSettings);
    //
    //     var request = new RestRequest(GetMethodName<T>(), method: Method.Post)
    //         .AddJsonBody(body, forceSerialize: false);
    //
    //     var response = await HandleRequest<SingleResponseWrapper<In.Message>>(request);
    //
    //     return response!.Ok;
    // }
    
    protected async Task<TResponse?> HandleRequest<TResponse>(RestRequest request)
    {
        try
        {
            var response = await ExecuteAsync(request);
            _logger.LogInformation("Request to {Url} executed", request.Resource);

            if (!response.IsSuccessStatusCode)
                ThrowResponseError(response);
            
            return JsonConvert.DeserializeObject<TResponse>(response.Content!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Request to {Url} failed", request.Resource);
            throw;
        }
    }

    private static void ThrowResponseError(RestResponse response)
    {
        var errorMessage = JsonConvert.DeserializeObject<ErrorResponse>(response.Content!)!;
        throw new Exception($"Request failed with code: {response.StatusCode}, error code: {errorMessage.ErrorCode}, error message: {errorMessage.ErrorMessage}");
    }
}