namespace Notifier.Blazor.Settings
{
    public record VkApiSettings(string ClientId, string ClientSecret, string Login, string Password, string AuthenticationSecret, string RedirectUrl)
    {
        public VkApiSettings() : this(default!, default!, default!, default!, default!, default!) { }
    }
}
