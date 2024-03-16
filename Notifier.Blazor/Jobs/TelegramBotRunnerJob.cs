using Notifier.Telegram.Contract;
using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Jobs
{
    internal class TelegramBotRunnerJob : Job<TelegramBotRunnerJob>
    {
        private readonly ITelegramBotUpdatesService _telegramBotService;

        public TelegramBotRunnerJob(ITelegramBotUpdatesService telegramBotService, ILogger<TelegramBotRunnerJob> logger, IMeterFactory meterFactory)
            : base(logger, meterFactory) 
        {
            _telegramBotService = telegramBotService;
        }

        protected override async Task Run()
        {
            await _telegramBotService.HandleUpdates();
        }
    }
}
