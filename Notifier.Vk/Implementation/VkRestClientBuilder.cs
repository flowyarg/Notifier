using Microsoft.Extensions.Logging;
using Notifier.Vk.Contract;

namespace Notifier.Vk.Implementation
{
    internal class VkRestClientBuilder : IVkRestClientBuilder
    {
        public VkRestClientBuilder(ILogger<VkRestClient> vkClientLogger)
        {
            _vkClientLogger = vkClientLogger;
        }
        private string? _accessToken;
        private readonly ILogger<VkRestClient> _vkClientLogger;

        public IVkRestClient Build()
        {
            if (_accessToken == null)
            {
                throw new InvalidOperationException("Access token was not specified");
            }

            return new VkRestClient(_accessToken, _vkClientLogger);
        }
        public IVkRestClientBuilder WithAccessToken(string accessToken)
        {
            _accessToken = accessToken;
            return this;
        }
    }
}
