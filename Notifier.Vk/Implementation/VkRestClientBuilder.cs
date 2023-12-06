using Notifier.Vk.Contract;

namespace Notifier.Vk.Implementation
{
    internal class VkRestClientBuilder : IVkRestClientBuilder
    {
        private string? _accessToken;
        public IVkRestClient Build()
        {
            if (_accessToken == null)
            {
                throw new InvalidOperationException("Access token was not specified");
            }

            return new VkRestClient(_accessToken);
        }
        public IVkRestClientBuilder WithAccessToken(string accessToken)
        {
            _accessToken = accessToken;
            return this;
        }
    }
}
