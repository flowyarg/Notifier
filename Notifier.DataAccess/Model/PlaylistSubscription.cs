using Microsoft.EntityFrameworkCore;

namespace Notifier.DataAccess.Model
{
    [PrimaryKey(nameof(SubscriberChatId), nameof(PlaylistOwnerId), nameof(PlaylistId))]
    public class PlaylistSubscription
    {
        public long SubscriberChatId {  get; set; }

        public int PlaylistId { get; set; }
        public int PlaylistOwnerId { get; set; }
        public virtual Playlist Playlist { get; set; }

        public DateTimeOffset? LastSyncDate { get; set; }
    }
}
