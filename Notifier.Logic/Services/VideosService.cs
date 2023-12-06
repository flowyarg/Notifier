using Microsoft.EntityFrameworkCore;
using Notifier.Logic.Models;
using Notifier.DataAccess;
using Notifier.DataAccess.Model;

namespace Notifier.Logic.Services
{
    public class VideosService
    {
        private readonly IDbContextFactory<NotifierDbContext> _factory;

        public VideosService(IDbContextFactory<NotifierDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyCollection<VideoInfo>> GetVideos(string? ownerId = null, string? playlistId = null, DateTimeOffset? uploadedAfter = null)
        {
            using var context = _factory.CreateDbContext();

            IQueryable<Video> query = context.Videos;

            if (ownerId != null)
            {
                query = query.Where(video => video.Playlist.Owner.OwnerId == ownerId);
            }

            if (playlistId != null)
            {
                query = query.Where(video => video.Playlist.PlaylistId == playlistId);
            }

            if (uploadedAfter != null)
            {
                query = query.Where(video => video.PublicationDate > uploadedAfter);
            }

            var videos = await query
                .Include(video => video.Playlist)
                .ThenInclude(playlist => playlist.Owner)
                .AsNoTracking()
                .ToListAsync();

            return videos.Select(video => new VideoInfo(
                Id: video.VideoId,
                OwnerId: video.Playlist.Owner.OwnerId,
                PlaylistId: video.Playlist.PlaylistId,
                Title: video.Title,
                Description: video.Description,
                PublicationDate: video.PublicationDate,
                Duration: video.Duration,
                Url: video.Url,
                PreviewUrl: video.PreviewUrl
                )).ToArray();
        }

        public async Task AddVideos(IReadOnlyCollection<VideoInfo> videos, string ownerId, string playlistId)
        {
            using var context = _factory.CreateDbContext();

            var realOwnerId = context.Owners.Single(owner => owner.OwnerId == ownerId).Id;
            var realPlaylistId = context.Playlists.Single(playlist => playlist.PlaylistId == playlistId).Id;

            context.Videos.AddRange(videos.Select(video => new DataAccess.Model.Video
            {
                OwnerId = realOwnerId,
                PlaylistId = realPlaylistId,
                VideoId = video.Id,
                Title = video.Title,
                Description = video.Description,
                PublicationDate = video.PublicationDate,
                Duration = video.Duration,
                Url = video.Url,
                PreviewUrl = video.PreviewUrl
            }));

            await context.SaveChangesAsync();
        }
    }
}
