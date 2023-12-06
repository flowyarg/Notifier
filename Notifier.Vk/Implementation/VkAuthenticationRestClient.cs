using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Notifier.Vk.Contract;
using Notifier.Vk.Models;
using RestSharp;

namespace Notifier.Vk.Implementation
{
    internal class VkAuthenticationRestClient : RestClient, IVkAuthenticationRestClient
    {
        private const string _apiEndpoint = "https://oauth.vk.com";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUrl;
        private readonly IReadOnlyCollection<string>? _scopes;

        public VkAuthenticationRestClient(string clientId, string clientSecret, string redirectUrl, IReadOnlyCollection<string>? scopes)
            : base(new RestClientOptions(_apiEndpoint))
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUrl = redirectUrl;
            _scopes = scopes;
        }

        public string GetAuthenticationQueryString()
        {
            if (_scopes == null || _scopes.Count == 0)
            {
                throw new InvalidOperationException("Scopes were not specified");
            }

            var parameters = new Dictionary<string, string?>() {
                { "client_id", _clientId },
                { "redirect_uri", _redirectUrl },
                { "response_type", "code" },
                { "display", "page" },
                { "scope", string.Join(", ", _scopes)}
            };

            var newUrl = new Uri(QueryHelpers.AddQueryString($"{_apiEndpoint}/authorize", parameters));
            return newUrl.ToString();
        }

        public async Task<AccessTokenResponse> GetAccessToken(string code)
        {
            var request = new RestRequest("access_token")
                .AddParameter("client_id", _clientId)
                .AddParameter("client_secret", _clientSecret)
                .AddParameter("redirect_uri", _redirectUrl)
                .AddParameter("code", code);

            var response = await this.GetAsync(request);

            return JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content!)!;
        }
    }
}
