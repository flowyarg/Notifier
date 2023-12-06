using Microsoft.Extensions.Logging;
using Notifier.Logic.Models;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Model;
using Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Services
{
    internal class PlaylistNotificationService : IPlaylistNotificationService
    {
        private const string _notificationText = """
            There is a new video in playlist "{0}": 
            """;

        private readonly VideosService _videosService;
        private readonly PlaylistsService _playlistsService;
        private readonly PlaylistSubscriptionService _playlistSubscriptionService;
        private readonly ITelegramRestClient _telegramRestClient;
        private readonly ILogger<PlaylistNotificationService> _logger;

        public PlaylistNotificationService(
            VideosService videosService,
            PlaylistsService playlistsService,
            PlaylistSubscriptionService playlistSubscriptionService,
            ITelegramRestClient telegramRestClient,
            ILogger<PlaylistNotificationService> logger)
        {
            _videosService = videosService;
            _playlistsService = playlistsService;
            _playlistSubscriptionService = playlistSubscriptionService;
            _telegramRestClient = telegramRestClient;
            _logger = logger;
        }

        public async Task Notify(string ownerId, string playlistId)
        {
            var subscribers = await _playlistSubscriptionService.GetSubscribers(playlistId);
            var playlist = await _playlistsService.GetPlaylist(ownerId, playlistId);

            var minSyncDate = subscribers.Any(subscriber => subscriber.LastSyncDate == null)
                ? null
                : subscribers.Min(subscriber => subscriber.LastSyncDate);

            var videos = await _videosService.GetVideos(ownerId,  playlistId, uploadedAfter: minSyncDate);

            if (videos.Count == 0)
            {
                return;
            }

            foreach (var (chatId, lastSyncDate) in subscribers)
            {
                var lastSuccessfulSyncDate = await ProcessSubscriber(videos, chatId, playlist, lastSyncDate);
                if (lastSuccessfulSyncDate != null)
                {
                    await _playlistSubscriptionService.UpdateLastSyncDate(chatId, playlistId, lastSuccessfulSyncDate.Value);
                }
            }
        }

        private async Task<DateTimeOffset?> ProcessSubscriber(IReadOnlyCollection<VideoInfo> videos, long chatId, VideoPlaylist playlist, DateTimeOffset? lastSyncDate)
        {
            var videosToSend = lastSyncDate == null ? videos : videos.Where(video => video.PublicationDate > lastSyncDate);

            DateTimeOffset? lastSyncedDate = null;
            try
            {
                foreach (var video in videosToSend)
                {
                    var notificationText = string.Format(_notificationText, playlist.Title);
                    await _telegramRestClient.SendMessage(new SendPhotoRequest
                    {
                        ChatId = chatId,
                        Caption = notificationText + video.Title,
                        PhotoUrl = video.PreviewUrl,
                        CaptionEntities = new[]
                        {
                            new MessageEntity
                            {
                                Type = Model.Incoming.MessageEntityType.TextLink,
                                Offset = _notificationText.Length - 6,
                                Length = playlist.Title.Length,
                                Url = playlist.Url,
                            },
                            new MessageEntity
                            {
                                Type = Model.Incoming.MessageEntityType.TextLink,
                                Offset = notificationText.Length,
                                Length = video.Title.Length,
                                Url = video.Url,
                            }
                        }
                    });

                    lastSyncedDate = video.PublicationDate;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Can't send all video notifications to chat {chatId}", chatId);
            }

            return lastSyncedDate;
        }
    }
}
