namespace Notifier.Telegram.Contract
{
    public interface IPlaylistNotificationService
    {
        Task Notify(string ownerId, string playlistId);
    }
}
