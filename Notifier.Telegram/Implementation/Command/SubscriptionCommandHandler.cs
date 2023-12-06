using Microsoft.Extensions.Logging;
using Notifier.Logic.Models;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Contract.Command;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Model.Incoming;
using Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Implementation.Command
{
    internal abstract class SubscriptionCommandHandler<T> : CommandHandler<T>
        where T : ICommandHandler
    {
        private const string _stopCommandOption = "__IM DONE HERE__";

        private readonly PlaylistsService _playlistsService;
        protected readonly PlaylistSubscriptionService _playlistSubscriptionService;
        protected readonly CommandContextStore? _commandContextStore;
        protected SubscriptionCommandHandler(
            ITelegramRestClient telegramClient,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            ILogger<T> logger,
            CommandContextStore? commandContextStore = null)
            : base(telegramClient, logger)
        {
            _playlistsService = playlistsService;
            _playlistSubscriptionService = playlistSubscriptionService;
            _commandContextStore = commandContextStore;
        }

        protected async Task<IReadOnlyCollection<VideoPlaylist>> GetExistingPlaylists(long chatId)
        {
            var playlists = await _playlistsService.GetPlaylists();

            var existingSubscriptionUrls = await _playlistSubscriptionService.GetSubscriptionUrls(chatId);

            return playlists.IntersectBy(existingSubscriptionUrls, playlist => playlist.Url).ToArray();
        }

        protected async Task<IReadOnlyCollection<VideoPlaylist>> GetMissingPlaylists(long chatId)
        {
            var playlists = await _playlistsService.GetPlaylists();

            var existingSubscriptionUrls = await _playlistSubscriptionService.GetSubscriptionUrls(chatId);

            return playlists.ExceptBy(existingSubscriptionUrls, playlist => playlist.Url).ToArray();
        }

        protected ReplyKeyboardMarkup CreateReplyKeyboard(IReadOnlyCollection<VideoPlaylist> playlists)
        {
            List<object[]> buttons = [];

            foreach (var playlistChunk in playlists.Chunk(3))
            {
                buttons.Add(playlistChunk.Select(p => new KeyboardButton { Text = p.Title }).ToArray());
            }

            buttons.Add([new KeyboardButton { Text = _stopCommandOption }]);

            return new ReplyKeyboardMarkup
            {
                InputFieldPlaceholder = "playlist name",
                Buttons = buttons,
                IsPersistent = true,
            };
        }

        public override async Task HandleReply(long chatId, Message message)
        {
            if (message.Text == _stopCommandOption)
            {
                await HandleReplyEndCommand(chatId);
                return;
            }

            var isPlaylistValid = await _playlistsService.Exists(message.Text!);
            if (!isPlaylistValid)
            {
                await HandleReplyMessage(chatId, message.Id, $"Playlist '{message.Text!}' does not exist");
                return;
            }

            await HandleReplySubscription(chatId, message.Id, message.Text!);
        }

        protected virtual Task HandleReplySubscription(long chatId, int messageId, string playlistName)
        {
            throw new NotImplementedException();
        }

        protected async Task HandleReplyEndCommand(long chatId)
        {
            var replyMessage = new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Ok",
                ReplyMarkup = new ReplyKeyboardRemove { RemoveKeyboard = true },
                DisableNotification = true,
            };

            await SendMessage(replyMessage);
            _commandContextStore!.StopCommandContext(chatId);
            _logger.LogInformation("{command} command reply handled (command context stopped)", _commandName.Value);
        }

        protected async Task HandleReplyMessage(long chatId, int replyToMessageId, string message)
        {
            var replyMessage = new SendMessageRequest
            {
                ChatId = chatId,
                Text = message,
                ReplyToMessageId = replyToMessageId,
                DisableNotification = true,
            };

            await SendMessage(replyMessage);
            _logger.LogInformation("{command} command reply handled", _commandName.Value);
        }
    }
}
