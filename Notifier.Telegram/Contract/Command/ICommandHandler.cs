using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Contract.Command
{
    public interface ICommandHandler
    {
        static virtual string Command => throw new NotImplementedException();
        static virtual string CommandDescription => throw new NotImplementedException();

        Task Handle(long chatId, string parameters);
        Task HandleReply(long chatId, Message message);
    }
}
