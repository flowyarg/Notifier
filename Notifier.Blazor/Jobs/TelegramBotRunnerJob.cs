using Coravel.Invocable;
using Notifier.Telegram.Contract;

namespace Notifier.Blazor.Jobs
{
    public class TelegramBotRunnerJob : IInvocable
    {
        private readonly ITelegramBotUpdatesService _telegramBotService;

        public TelegramBotRunnerJob(ITelegramBotUpdatesService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task Invoke()
        {
            await _telegramBotService.HandleUpdates();
        }
    }
}
