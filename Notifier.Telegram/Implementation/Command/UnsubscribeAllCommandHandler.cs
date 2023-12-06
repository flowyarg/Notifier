using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;

namespace Notifier.Telegram.Implementation.Command
{
    internal class UnsubscribeAllCommandHandler : SubscriptionCommandHandler<UnsubscribeAllCommandHandler>
    {
        public static string Command => "/unsubscribe_all";
        public static string CommandDescription => "unsubscribe from notifications in all playlists";

        public UnsubscribeAllCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            ILogger<UnsubscribeAllCommandHandler> logger)
            : base(telegramClient, playlistsService, playlistSubscriptionService, logger)
        {}

        public override async Task Handle(long chatId, string parameters)
        {
            var existingPlaylists = await GetExistingPlaylists(chatId);

            await _playlistSubscriptionService.UnsubscribeFrom(chatId, existingPlaylists.Select(playlist => playlist.Title).ToArray());

            var message = TelegramResponseMessageBuilder
                .New
                .WithChatId(chatId)
                .AddText("You have unsubscribed from all existing playlists, please check your subscriptions with command ")
                .AddCommand("/my_subscriptions")
                .Notify(false)
                .BuildMessage();

            await SendMessage(message);
            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
