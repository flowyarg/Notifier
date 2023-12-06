using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Implementation.Command
{
    internal class UnsubscribeCommandHandler : SubscriptionCommandHandler<UnsubscribeCommandHandler>
    {
        public static string Command => "/unsubscribe";
        public static string CommandDescription => "unsubscribe from notifications in certain playlist";

        public UnsubscribeCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            CommandContextStore commandContextStore,
            ILogger<UnsubscribeCommandHandler> logger)
            : base(telegramClient, playlistsService, playlistSubscriptionService, logger, commandContextStore)
        {}

        public override async Task Handle(long chatId, string parameters)
        {
            var existingPlaylists = await GetExistingPlaylists(chatId);

            var replyKeyboard = CreateReplyKeyboard(existingPlaylists);

            var messageToSend = new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Use buttons below to select playlists to unsubscribe from",
                ReplyMarkup = replyKeyboard,
                DisableNotification = true,
            };

            await SendMessage(messageToSend);
            _commandContextStore!.StartCommandContext(chatId, Command);

            _logger.LogInformation("{command} command handled", Command);
        }

        protected override async Task HandleReplySubscription(long chatId, int messageId, string playlistName)
        {
            var isAlreadySubscribed = await _playlistSubscriptionService.IsSubscribedTo(chatId, playlistName);

            if (!isAlreadySubscribed)
            {
                await HandleReplyMessage(chatId, messageId, $"You are not subscribed to '{playlistName}' playlist anyway");
                return;
            }

            await _playlistSubscriptionService.UnsubscribeFrom(chatId, playlistName);

            await HandleReplyMessage(chatId, messageId, $"You have unsubscribed from '{playlistName}' playlist");
        }
    }
}
