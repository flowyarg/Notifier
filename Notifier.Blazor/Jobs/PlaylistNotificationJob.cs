using Coravel.Invocable;
using Notifier.Telegram.Contract;
using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Jobs
{
    internal class PlaylistNotificationJob : Job<PlaylistNotificationJob>, IInvocableWithPayload<(string OwnerId, string PlaylistId)>
    {
        private readonly IPlaylistNotificationService _playlistNotificationService;

        public (string OwnerId, string PlaylistId) Payload { get; set; }

        public PlaylistNotificationJob(IPlaylistNotificationService playlistNotificationService, ILogger<PlaylistNotificationJob> logger, IMeterFactory meterFactory)
            : base(logger, meterFactory)
        {
            _playlistNotificationService = playlistNotificationService;
        }

        protected override async Task Run()
        {
            await _playlistNotificationService.Notify(Payload.OwnerId, Payload.PlaylistId);
        }
    }
}
