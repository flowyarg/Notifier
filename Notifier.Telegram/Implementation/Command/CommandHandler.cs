using Microsoft.Extensions.Logging;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Contract.Command;
using Notifier.Telegram.Model.Incoming;
using Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Implementation.Command
{
    internal abstract class CommandHandler<T> : ICommandHandler
        where T : ICommandHandler
    {
        protected readonly ITelegramRestClient _telegramClient;
        protected readonly ILogger<T> _logger;
        protected readonly Lazy<string> _commandName = new(() => (string)typeof(T).GetProperty(nameof(ICommandHandler.Command))!.GetValue(null)!);

        protected CommandHandler(ITelegramRestClient telegramClient, ILogger<T> logger)
        {
            _telegramClient = telegramClient;
            _logger = logger;
        }

        public abstract Task Handle(long chatId, string parameters);

        public virtual Task HandleReply(long chatId, Message message) => throw new NotImplementedException();

        protected async Task SendMessage<TMessage>(TMessage message)
            where TMessage : SendRequest
        {
            try
            {
                var sent = await _telegramClient.SendMessage(message);
                if (!sent)
                {
                    _logger.LogWarning("{command} command send message failed", _commandName.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{command} command send message resulted an error", _commandName.Value);
            }
        }
    }
}
