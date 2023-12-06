using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal interface IUpdateHandler
    {
        IUpdateHandler? NextHandler { get; set; }
        Task<bool> Handle(Update update);
    }
}
