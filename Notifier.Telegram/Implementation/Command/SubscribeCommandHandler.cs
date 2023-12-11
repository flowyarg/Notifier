using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Implementation.Command
{
    internal class SubscribeCommandHandler : SubscriptionCommandHandler<SubscribeCommandHandler>
    {
        public static string Command => "/subscribe";
        public static string CommandDescription => "subscribe to notifications in certain playlist";

        public SubscribeCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            CommandContextStore commandContextStore,
            ILogger<SubscribeCommandHandler> logger)
            : base(telegramClient, playlistsService, playlistSubscriptionService, logger, Command, commandContextStore)
        {}

        public override async Task Handle(long chatId, string parameters)
        {
            _logger.LogInformation("{command} command received", Command);

            var missingPlaylists = await GetMissingPlaylists(chatId);

            var replyKeyboard = CreateReplyKeyboard(missingPlaylists);

            var messageToSend = new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Use buttons below to select playlists to subscribe to",
                ReplyMarkup = replyKeyboard,
                DisableNotification = true,
            };

            await SendMessage(messageToSend);
            _commandContextStore!.StartCommandContext(chatId, Command);

            _logger.LogInformation("{command} command handled (command context started)", Command);
        }

        protected override async Task HandleReplySubscription(long chatId, int messageId, string playlistName)
        {
            var isAlreadySubscribed = await _playlistSubscriptionService.IsSubscribedTo(chatId, playlistName);

            if (isAlreadySubscribed)
            {
                await HandleReplyMessage(chatId, messageId, $"You are already subscribed to '{playlistName}' playlist");
                return;
            }

            await _playlistSubscriptionService.SubscribeTo(chatId, playlistName, DateTimeOffset.Now);

            await HandleReplyMessage(chatId, messageId, $"You have subscribed to '{playlistName}' playlist");
        }
    }
}
