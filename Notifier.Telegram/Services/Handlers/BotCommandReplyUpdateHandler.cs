using Microsoft.Extensions.Logging;
using Notifier.Telegram.DI;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal class BotCommandReplyUpdateHandler : UpdateHandler
    {
        private readonly CommandHandlerResolver _commandHandlerResolver;
        private readonly CommandContextStore _commandContextStore;
        private readonly ILogger<BotCommandReplyUpdateHandler> _logger;

        public BotCommandReplyUpdateHandler(CommandHandlerResolver commandHandlerResolver, CommandContextStore commandContextStore, ILogger<BotCommandReplyUpdateHandler> logger)
        {
            _commandHandlerResolver = commandHandlerResolver;
            _commandContextStore = commandContextStore;
            _logger = logger;
        }

        protected override bool CanHandle(Update update)
        {
            if (update.Message == null)
            {
                return false;
            }

            return _commandContextStore.GetCommandContext(update.Message.Chat.Id) != null;
        }
        protected override async Task<bool> HandleInternal(Update update)
        {
            try
            {
                var command = _commandContextStore.GetCommandContext(update.Message!.Chat.Id)!;

                var handler = _commandHandlerResolver(command);

                await handler.HandleReply(update.Message!.Chat.Id, update.Message!);
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Command reply handling error");
                return false;
            }
        }
    }
}
