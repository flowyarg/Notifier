using Notifier.Telegram.Contract;

namespace Notifier.Blazor.Jobs
{
    internal class TelegramBotRunnerJob : Job<TelegramBotRunnerJob>
    {
        private readonly ITelegramBotUpdatesService _telegramBotService;

        public TelegramBotRunnerJob(ITelegramBotUpdatesService telegramBotService, ILogger<TelegramBotRunnerJob> logger)
            : base(logger) 
        {
            _telegramBotService = telegramBotService;
        }

        protected override async Task Run()
        {
            await _telegramBotService.HandleUpdates();
        }
    }
}
