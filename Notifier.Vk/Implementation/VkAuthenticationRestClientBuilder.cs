using Notifier.Vk.Contract;

namespace Notifier.Vk.Implementation
{
    internal class VkAuthenticationRestClientBuilder : IVkAuthenticationRestClientBuilder
    {
        private string? _clientId;
        private string? _clientSecret;
        private string? _redirectUrl;
        private readonly List<string> _scopes = [];
        public IVkAuthenticationRestClient Build()
        {
            if (_clientSecret == null)
            {
                throw new InvalidOperationException("Client secret was not specified");
            }
            if (_clientId == null)
            {
                throw new InvalidOperationException("Client id was not specified");
            }
            if (_redirectUrl == null)
            {
                throw new InvalidOperationException("Redirect url was not specified");
            }

            return new VkAuthenticationRestClient(_clientId, _clientSecret, _redirectUrl, _scopes);
        }

        public IVkAuthenticationRestClientBuilder WithClientId(string clientId)
        {
            _clientId = clientId;
            return this;
        }

        public IVkAuthenticationRestClientBuilder WithClientSecret(string clientSecret)
        {
            _clientSecret = clientSecret;
            return this;
        }

        public IVkAuthenticationRestClientBuilder WithScope(string scope)
        {
            _scopes.Add(scope);
            return this;
        }

        public IVkAuthenticationRestClientBuilder WithRedirectUrl(string redirectUrl)
        {
            _redirectUrl = redirectUrl;
            return this;
        }
    }
}
