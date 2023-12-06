using Microsoft.EntityFrameworkCore;
using Notifier.DataAccess;
using Notifier.DataAccess.Model;
using System.Linq;

namespace Notifier.Logic.Services
{
    public class PlaylistSubscriptionService
    {
        private readonly IDbContextFactory<NotifierDbContext> _factory;
        public PlaylistSubscriptionService(IDbContextFactory<NotifierDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> IsSubscribedTo(long chatId, string playlistTitle)
        {
            using var context = _factory.CreateDbContext();

            return await context.Subscriptions
                .Include(subscription => subscription.Playlist)
                .Where(subscription => subscription.SubscriberChatId == chatId)
                .Where(subscription => subscription.Playlist.Title == playlistTitle)
                .SingleOrDefaultAsync() != null;
        }

        public async Task SubscribeTo(long chatId, string playlistTitle, DateTimeOffset? fromDate = null)
        {
            using var context = _factory.CreateDbContext();
            
            var realPlaylist = await context.Playlists.SingleAsync(playlist => playlist.Title == playlistTitle);

            context.Subscriptions.Add(new PlaylistSubscription
            {
                PlaylistId = realPlaylist.Id,
                PlaylistOwnerId = realPlaylist.OwnerId,
                SubscriberChatId = chatId,
                LastSyncDate = fromDate,
            });

            await context.SaveChangesAsync();
        }

        public async Task SubscribeTo(long chatId, IReadOnlyCollection<string> playlistTitles, DateTimeOffset? fromDate = null)
        {
            using var context = _factory.CreateDbContext();

            var realPlaylists = await context.Playlists.Where(playlist => playlistTitles.Contains(playlist.Title)).ToArrayAsync();

            context.Subscriptions.AddRange(realPlaylists.Select(playlist => new PlaylistSubscription
            {
                PlaylistId = playlist.Id,
                PlaylistOwnerId = playlist.OwnerId,
                SubscriberChatId = chatId,
                LastSyncDate = fromDate,
            }));

            await context.SaveChangesAsync();
        }

        public async Task UnsubscribeFrom(long chatId, string playlistTitle)
        {
            using var context = _factory.CreateDbContext();

            var realPlaylist = await context.Playlists.SingleAsync(playlist => playlist.Title == playlistTitle);

            var subscription = await context.Subscriptions
                .Where(subscription => subscription.SubscriberChatId == chatId)
                .Where(subscription => subscription.PlaylistId == realPlaylist.Id)
                .Where(subscription => subscription.PlaylistOwnerId == realPlaylist.OwnerId)
                .SingleAsync();

            context.Subscriptions.Remove(subscription);

            await context.SaveChangesAsync();
        }

        public async Task UnsubscribeFrom(long chatId, IReadOnlyCollection<string> playlistTitles)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var realPlaylists = await context.Playlists.Where(playlist => playlistTitles.Contains(playlist.Title)).ToArrayAsync();

                var realPlaylistIds = realPlaylists.Select(playlist => playlist.Id).ToArray();

                var subscriptions = await context.Subscriptions
                    .Where(subscription => subscription.SubscriberChatId == chatId)
                    .Where(subscription => realPlaylistIds.Contains(subscription.Playlist.Id))
                    .ToArrayAsync();

                context.Subscriptions.RemoveRange(subscriptions);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var ttttt = ex;
                throw;
            }
        }

        public async Task<IReadOnlyCollection<(long ChatId, DateTimeOffset? LastSyncDate)>> GetSubscribers(string playlistId)
        {
            using var context = _factory.CreateDbContext();

            var subscriptions = await context.Subscriptions
                .Include(subscription => subscription.Playlist)
                .Where(subscription => subscription.Playlist.PlaylistId == playlistId)
                .AsNoTracking()
                .Select(subscription => new { subscription.SubscriberChatId, subscription.LastSyncDate })
                .ToArrayAsync();

            return subscriptions
                .Select(subscription => (subscription.SubscriberChatId, subscription.LastSyncDate))
                .ToArray();
        }

        public async Task UpdateLastSyncDate(long chatId, string playlistId, DateTimeOffset lastSyncDate)
        {
            using var context = _factory.CreateDbContext();

            var subscription = await context.Subscriptions
               .Include(subscription => subscription.Playlist)
               .Where(subscription => subscription.SubscriberChatId == chatId)
               .Where(subscription => subscription.Playlist.PlaylistId == playlistId)
               .SingleAsync();

            subscription.LastSyncDate = lastSyncDate;
            await context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<string>> GetSubscriptionUrls(long chatId)
        {
            using var context = _factory.CreateDbContext();

            var subscriptions = await context.Subscriptions
                .Where(subscription => subscription.SubscriberChatId == chatId)
                .Include(subscription => subscription.Playlist)
                .AsNoTracking()
                .ToArrayAsync();

            return subscriptions
                .Select(subscription => subscription.Playlist.Url)
                .ToArray();
        }
    }
}
