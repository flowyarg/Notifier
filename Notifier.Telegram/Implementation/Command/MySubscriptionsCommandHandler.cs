using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;

namespace Notifier.Telegram.Implementation.Command
{
    internal class MySubscriptionsCommandHandler : CommandHandler<MySubscriptionsCommandHandler>
    {
        public static string Command => "/my_subscriptions";
        public static string CommandDescription => "list all playlists i am subscribed to";

        private readonly PlaylistsService _playlistsService;
        private readonly PlaylistSubscriptionService _playlistSubscriptionService;

        public MySubscriptionsCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            ILogger<MySubscriptionsCommandHandler> logger)
            : base(telegramClient, logger)
        {
            _playlistsService = playlistsService;
            _playlistSubscriptionService = playlistSubscriptionService;
        }

        public override async Task Handle(long chatId, string parameters)
        {
            var playlists = await _playlistsService.GetPlaylists();

            var existingSubscriptionUrls = await _playlistSubscriptionService.GetSubscriptionUrls(chatId);

            playlists = playlists.IntersectBy(existingSubscriptionUrls, playlist => playlist.Url).ToArray();

            var messageBuilder = TelegramResponseMessageBuilder
                .New
                .WithChatId(chatId)
                .Notify(false)
                .AddLine("You are subscribed to these playlists:", bold: true, underlined: true);

            foreach(var playlist in playlists)
            {
                messageBuilder
                    .AddText("▶ ")
                    .AddLink(playlist.Title, playlist.Url)
                    .AddText(" (by ")
                    .AddLink(playlist.Owner.DisplayName, playlist.Owner.Url)
                    .AddLine(");");
            }

            await SendMessage(messageBuilder.BuildMessage());
            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
