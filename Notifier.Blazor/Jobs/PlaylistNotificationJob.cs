using Coravel.Invocable;
using Notifier.Telegram.Contract;

namespace Notifier.Blazor.Jobs
{
    internal class PlaylistNotificationJob : IInvocable, IInvocableWithPayload<(string OwnerId, string PlaylistId)>
    {
        private readonly IPlaylistNotificationService _playlistNotificationService;

        public (string OwnerId, string PlaylistId) Payload { get; set; }

        public PlaylistNotificationJob(IPlaylistNotificationService playlistNotificationService)
        {
            _playlistNotificationService = playlistNotificationService;
        }

        public async Task Invoke()
        {
            await _playlistNotificationService.Notify(Payload.OwnerId, Payload.PlaylistId);
        }
    }
}
