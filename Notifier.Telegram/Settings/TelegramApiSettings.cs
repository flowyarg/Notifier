namespace Notifier.Telegram.Settings
{
    public record TelegramApiSettings(string BotName, string AccessToken)
    {
        public TelegramApiSettings() : this(default!, default!) { }
    }
}
