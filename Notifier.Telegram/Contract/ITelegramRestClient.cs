using In = Notifier.Telegram.Model.Incoming;
using Out = Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Contract
{
    public interface ITelegramRestClient
    {
        Task<IReadOnlyCollection<In.Update>> GetUpdates(int timeout = 0, int limit = 100, int? offset = null);
        Task<bool> SendMessage<T>(T message) where T : Out.SendRequest;
    }
}
