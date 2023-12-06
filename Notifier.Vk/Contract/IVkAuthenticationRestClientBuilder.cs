namespace Notifier.Vk.Contract
{
    public interface IVkAuthenticationRestClientBuilder
    {
        IVkAuthenticationRestClient Build();

        IVkAuthenticationRestClientBuilder WithClientId(string clientId);
        IVkAuthenticationRestClientBuilder WithClientSecret(string clientSecret);
        IVkAuthenticationRestClientBuilder WithScope(string scope);
        IVkAuthenticationRestClientBuilder WithRedirectUrl(string redirectUrl);
    }
}
