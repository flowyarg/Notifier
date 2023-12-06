using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Services.Handlers;

namespace Notifier.Telegram.Services
{
    internal class TelegramBotUpdatesService : ITelegramBotUpdatesService
    {
        private readonly ITelegramRestClient _telegramClient;
        private readonly IUpdateHandler _updateHandler;
        private readonly ILogger<TelegramBotUpdatesService> _logger;

        public TelegramBotUpdatesService(ITelegramRestClient telegramClient, IUpdateHandler updateHandler, ILogger<TelegramBotUpdatesService> logger)
        {
            _telegramClient = telegramClient;
            _updateHandler = updateHandler;
            _logger = logger;
        }

        public async Task HandleUpdates()
        {
            int? lastProcessedUpdateId = null;

            for (; ; )
            {
                var updates = await _telegramClient.GetUpdates(offset: lastProcessedUpdateId + 1, limit: 100, timeout: 60);

                if (updates.Count == 0)
                {
                    return;
                }

                foreach (var update in updates)
                {
                    try
                    {
                        var wasHandled = await _updateHandler.Handle(update);
                        if (!wasHandled)
                        {
                            _logger.LogWarning("Update was not handled by any handler. Skipping update: {update}", JsonConvert.SerializeObject(update));
                        }
                    }
                    catch (Exception)
                    {
                        //TODO
                        throw;
                    }
                    finally
                    {
                        lastProcessedUpdateId = update.Id;
                    }
                }
            }
        }
    }
}
