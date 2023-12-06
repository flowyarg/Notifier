namespace Notifier.Vk.Contract
{
    public interface IVkRestClientBuilder
    {
        IVkRestClient Build();

        IVkRestClientBuilder WithAccessToken(string accessToken);
    }
}
