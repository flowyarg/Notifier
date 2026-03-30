namespace Notifier.Blazor.Jobs
{
    using Coravel.Queuing.Interfaces;
    using Logic.Extensions;
    using Logic.Models;
    using Logic.Services;
    using Vk.Contract;

    internal class SyncPlaylistsJob : Job<SyncPlaylistsJob>
    {
        private readonly PlaylistsService _playlistsService;
        private readonly VideosService _videosService;
        private readonly IVkVideoRestClient _vkVideoRestClient;
        private readonly IQueue _queue;

        public SyncPlaylistsJob(
            PlaylistsService playlistsService,
            VideosService videosService,
            IQueue queue,
            ILogger<SyncPlaylistsJob> logger,
            IVkVideoRestClient vkVideoRestClient)
            : base(logger)
        {
            _playlistsService = playlistsService;
            _videosService = videosService;
            _queue = queue;
            _vkVideoRestClient = vkVideoRestClient;
        }

        protected override async Task Run()
        {
            var playlistsToSync = await _playlistsService.GetTrackedPlaylists();

            if (playlistsToSync.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Starting to sync {Count} playlists", playlistsToSync.Count);

            foreach (var playlist in playlistsToSync)
            {
                _logger.LogInformation("Starting to sync playlist '{Playlist}'", playlist.Title);
                List<VideoInfo> videosToAdd = [];
                var existingVideos = await _videosService.GetVideos(playlist.Owner.Id, playlist.Id);

                var existingIds = existingVideos.Select(video => video.Id).ToHashSet();

                await foreach (var vkVideo in _vkVideoRestClient.GetVideos(playlist.Owner.Id, playlist.Owner.OwnerType.ToVk(), playlist.Id))
                {
                    if (existingIds.Contains(vkVideo.Id))
                    {
                        continue;
                    }

                    var previewUrl = vkVideo.VideoPreviews!
                        .OrderByDescending(preview => preview.Width * preview.Height)
                        .First()
                        .ImageUrl;

                    videosToAdd.Add(new VideoInfo
                    (
                        Id: vkVideo.Id,
                        OwnerId: playlist.Owner.Id,
                        PlaylistId: playlist.Id,
                        Title: vkVideo.Title,
                        Description: vkVideo.Description,
                        PublicationDate: vkVideo.PublicationDate,
                        Duration: TimeSpan.FromSeconds(vkVideo.DurationSeconds),
                        Url: vkVideo.Url,
                        PreviewUrl: previewUrl
                    ));
                }

                if (videosToAdd.Count == 0)
                {
                    continue;
                }

                await _videosService.AddVideos(videosToAdd, playlist.Owner.Id, playlist.Id);

                _logger.LogInformation("{Count} videos added to playlist {Playlist}", videosToAdd.Count, playlist.Title);

                _queue.QueueInvocableWithPayload<PlaylistNotificationJob, (string OwnerId, string PlaylistId)>((playlist.Owner.Id, playlist.Id));
            }
        }
    }
}
