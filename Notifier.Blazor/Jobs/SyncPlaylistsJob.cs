﻿using Coravel.Queuing.Interfaces;
using Notifier.Logic.Extensions;
using Notifier.Logic.Models;
using Notifier.Logic.Services;
using Notifier.Vk.Contract;
using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Jobs
{
    internal class SyncPlaylistsJob : Job<SyncPlaylistsJob>
    {
        private readonly AccessTokenService _accessTokenService;
        private readonly PlaylistsService _playlistsService;
        private readonly VideosService _videosService;
        private readonly IVkRestClientBuilder _vkRestClientBuilder;
        private readonly IQueue _queue;

        public SyncPlaylistsJob(
            AccessTokenService accessTokenService,
            PlaylistsService playlistsService,
            VideosService videosService,
            IVkRestClientBuilder vkRestClientBuilder,
            IQueue queue,
            ILogger<SyncPlaylistsJob> logger,
            IMeterFactory meterFactory)
            : base(logger, meterFactory)
        {
            _accessTokenService = accessTokenService;
            _playlistsService = playlistsService;
            _videosService = videosService;
            _vkRestClientBuilder = vkRestClientBuilder;
            _queue = queue;
        }

        protected override async Task Run()
        {
            var playlistsToSync = await _playlistsService.GetTrackedPlaylists();

            if (playlistsToSync.Count == 0)
            {
                return;
            }

            var (accessToken, validThrough) = await _accessTokenService.GetAccessToken();

            if (validThrough < DateTimeOffset.Now)
            {
                throw new Exception("Vk token has expired");
            }

            using var vkClient = _vkRestClientBuilder
                .WithAccessToken(accessToken)
                .Build();

            _logger.LogInformation("Starting to sync {Count} playlists", playlistsToSync.Count);

            foreach (var playlist in playlistsToSync)
            {
                _logger.LogInformation("Starting to sync playlist '{Playlist}'", playlist.Title);
                List<VideoInfo> videosToAdd = [];
                var existingVideos = await _videosService.GetVideos(playlist.Owner.Id, playlist.Id);

                var existingIds = existingVideos.Select(video => video.Id).ToHashSet();

                await foreach (var vkVideo in vkClient.GetVideos(playlist.Owner.Id, playlist.Owner.OwnerType.ToVk(), playlist.Id))
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
