using Notifier.Vk.Models;

namespace Notifier.Vk.Contract
{
    public interface IVkAuthenticationRestClient : IDisposable
    {
        string GetAuthenticationQueryString();
        Task<AccessTokenResponse> GetAccessToken(string code);
    }
}
