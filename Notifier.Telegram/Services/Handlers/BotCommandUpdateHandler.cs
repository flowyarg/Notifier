using Microsoft.Extensions.Logging;
using Notifier.Telegram.DI;
using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal class BotCommandUpdateHandler : UpdateHandler
    {
        private readonly CommandHandlerResolver _commandHandlerResolver;
        private readonly ILogger<BotCommandUpdateHandler> _logger;

        public BotCommandUpdateHandler(CommandHandlerResolver commandHandlerResolver, ILogger<BotCommandUpdateHandler> logger)
        {
            _commandHandlerResolver = commandHandlerResolver;
            _logger = logger;
        }

        protected override bool CanHandle(Update update)
        {
            if (update.Message == null)
            {
                return false;
            }

            if (update.CallbackQuery != null)
            {
                return false;
            }

            if (update.Message.MessageEntities == null || update.Message.MessageEntities.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(update.Message.Text))
            {
                return false;
            }

            if (update.Message.MessageEntities.Any(entity => entity.Type == MessageEntityType.BotCommand))
            {
                return true;
            }

            return false;
        }

        protected override async Task<bool> HandleInternal(Update update)
        {
            try
            {
                var commandEntity = update!.Message!.MessageEntities!.First(entity => entity.Type == MessageEntityType.BotCommand);
                var command = update!.Message!.Text![commandEntity.Offset..(commandEntity.Offset + commandEntity.Length)];
                var commandParameters = update!.Message!.Text![(commandEntity.Offset + commandEntity.Length)..];

                var handler = _commandHandlerResolver(command);

                await handler.Handle(update.Message!.Chat.Id, commandParameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Command handling error");
                return false;
            }
        }
    }
}
