using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;

namespace Notifier.Telegram.Implementation.Command
{
    internal class SubscribeAllCommandHandler : SubscriptionCommandHandler<SubscribeAllCommandHandler>
    {
        public static string Command => "/subscribe_all";
        public static string CommandDescription => "subscribe to notifications in all playlists";

        public SubscribeAllCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            ILogger<SubscribeAllCommandHandler> logger)
            : base(telegramClient, playlistsService, playlistSubscriptionService, logger, Command)
        {}

        public override async Task Handle(long chatId, string parameters)
        {
            _logger.LogInformation("{command} command received", Command);

            var missingPlaylists = await GetMissingPlaylists(chatId);

            await _playlistSubscriptionService.SubscribeTo(chatId, missingPlaylists.Select(playlist => playlist.Title).ToArray(), DateTimeOffset.Now);

            var message = TelegramResponseMessageBuilder
                .New
                .WithChatId(chatId)
                .AddText("You have subscribed to all existing playlists, please check your subscriptions with command ")
                .AddCommand("/my_subscriptions")
                .Notify(false)
                .BuildMessage();

            await SendMessage(message);
            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
