namespace Notifier.Vk.Implementation;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using RestSharp;

public class VkVideoAuthRestClient : RestClient
{
    private readonly ILogger<VkVideoAuthRestClient> _logger;

    public VkVideoAuthRestClient(ILogger<VkVideoAuthRestClient> logger) : base(new RestClientOptions("https://login.vk.com"))
    {
        _logger = logger;
    }

    public async Task<string> GetAccessToken(string clientSecret, string clientId, string scopes, string appId)
    {
        var request = new RestRequest("/", Method.Post)
            .AddQueryParameter("act", "get_anonym_token")
            .AddParameter("client_secret", clientSecret)
            .AddParameter("client_id", clientId)
            .AddParameter("scopes", scopes)
            .AddParameter("isApiOauthAnonymEnabled", "false")
            .AddParameter("version", "1")
            .AddParameter("app_id", appId);
        
        var response = await this.PostAsync(request);
        _logger.LogInformation("Request to {Url} executed", request.Resource);
        using var jsonDocument = JsonDocument.Parse(response.Content!);
        var token = jsonDocument.RootElement.GetProperty("data").GetProperty("access_token").GetString()!;
        return token;
    }
}