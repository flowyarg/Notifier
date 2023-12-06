namespace Notifier.Telegram.Contract
{
    public interface ITelegramBotUpdatesService
    {
        Task HandleUpdates();
    }
}
