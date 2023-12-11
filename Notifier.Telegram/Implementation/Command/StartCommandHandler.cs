using Microsoft.Extensions.Logging;
using Notifier.Telegram.Contract;

namespace Notifier.Telegram.Implementation.Command
{
    internal class StartCommandHandler : CommandHandler<StartCommandHandler>
    {
        public static string Command => "/start";
        public static string CommandDescription => "start bot and show help message";

        private readonly HelpCommandHandler _helpCommandHandler;

        public StartCommandHandler(ITelegramRestClient telegramClient, HelpCommandHandler helpCommandHandler, ILogger<StartCommandHandler> logger)
            : base(telegramClient, logger)
        {
            _helpCommandHandler = helpCommandHandler;
        }

        public override async Task Handle(long chatId, string parameters)
        {
            _logger.LogInformation("{command} command received", Command);

            await _helpCommandHandler.Handle(chatId, parameters);

            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
