using Microsoft.EntityFrameworkCore;
using Notifier.DataAccess;
using Notifier.DataAccess.Migrations;
using Notifier.DataAccess.Model;
using Model = Notifier.Logic.Models;

namespace Notifier.Logic.Services
{
    public class PlaylistsService
    {
        private readonly IDbContextFactory<NotifierDbContext> _factory;
        private readonly OwnersService _ownersService;

        public PlaylistsService(IDbContextFactory<NotifierDbContext> factory, OwnersService ownersService)
        {
            _factory = factory;
            _ownersService = ownersService;
        }

        public async Task<IReadOnlyCollection<Model.VideoPlaylist>> GetPlaylists()
        {
            using var context = _factory.CreateDbContext();
            var playlists = await context.Playlists
                .Include(playlist => playlist.Owner)
                .OrderBy(playlist => playlist.Owner.OwnerId)
                .ThenBy(playlist => playlist.Title)
                .AsNoTracking()
                .ToListAsync();

            var owners = (await _ownersService.GetOwners(playlists.Select(p => p.Owner.OwnerId).ToArray()))
                .ToDictionary(owner => owner.Id, owner => owner);

            return playlists.Select(playlist => new Model.VideoPlaylist(
                    Id: playlist.PlaylistId,
                    Owner: owners[playlist.Owner.OwnerId],
                    Title: playlist.Title,
                    Url: playlist.Url,
                    IsBeingTracked: playlist.IsBeingTracked
                    )).ToArray();
        }

        public async Task<IReadOnlyCollection<Model.VideoPlaylist>> GetPlaylists(string ownerId)
        {
            using var context = _factory.CreateDbContext();
            var playlists = await context.Playlists.Where(playlist => playlist.Owner.OwnerId == ownerId)
                .Include(playlist => playlist.Owner)
                .OrderBy(playlist => playlist.Owner.OwnerId)
                .ThenBy(playlist => playlist.Title)
                .AsNoTracking()
                .ToListAsync();

            var owners = (await _ownersService.GetOwners(playlists.Select(p => p.Owner.OwnerId).ToArray()))
                .ToDictionary(owner => owner.Id, owner => owner);

            return playlists.Select(playlist => new Model.VideoPlaylist(
                   Id: playlist.PlaylistId,
                   Owner: owners[playlist.Owner.OwnerId],
                   Title: playlist.Title,
                   Url: playlist.Url,
                   IsBeingTracked: playlist.IsBeingTracked
                   )).ToArray();
        }

        public async Task<IReadOnlyCollection<Model.VideoPlaylist>> GetTrackedPlaylists()
        {
            using var context = _factory.CreateDbContext();
            var playlists = await context.Playlists.Where(playlist => playlist.IsBeingTracked)
                .Include(playlist => playlist.Owner)
                .OrderBy(playlist => playlist.Owner.OwnerId)
                .ThenBy(playlist => playlist.Title)
                .AsNoTracking()
                .ToListAsync();

            var owners = (await _ownersService.GetOwners(playlists.Select(p => p.Owner.OwnerId).ToArray()))
                .ToDictionary(owner => owner.Id, owner => owner);

            return playlists.Select(playlist => new Model.VideoPlaylist(
                   Id: playlist.PlaylistId,
                   Owner: owners[playlist.Owner.OwnerId],
                   Title: playlist.Title,
                   Url: playlist.Url,
                   IsBeingTracked: playlist.IsBeingTracked
                   )).ToArray();
        }

        public async Task<bool> Exists(string playlistTitle)
        {
            using var context = _factory.CreateDbContext();

            return await context.Playlists.AnyAsync(playlist => playlist.Title == playlistTitle);
        }

        public async Task<Model.VideoPlaylist> GetPlaylist(string ownerId, string playlistId)
        {
            using var context = _factory.CreateDbContext();

            var playlist = await context.Playlists.Include(playlist => playlist.Owner).SingleAsync(playlist => playlist.Owner.OwnerId == ownerId && playlist.PlaylistId == playlistId);

            var owner = (await _ownersService.GetOwners([playlist.Owner.OwnerId])).Single();

            return new Model.VideoPlaylist(
                 Id: playlist.PlaylistId,
                 Owner: owner,
                 Title: playlist.Title,
                 Url: playlist.Url,
                 IsBeingTracked: playlist.IsBeingTracked);
        }

        public async Task AddPlaylist(string ownerId, string playlistId, string title, string url)
        {
            using var context = _factory.CreateDbContext();
            var owner = context.Owners.Single(owner => owner.OwnerId == ownerId);
            context.Playlists.Add(new Playlist { Owner = owner, PlaylistId = playlistId, Title = title, Url = url });
            await context.SaveChangesAsync();
        }

        public async Task DeletePlaylist(string playlistId)
        {
            using var context = _factory.CreateDbContext();
            var playlist = context.Playlists.SingleOrDefault(playlist => playlist.PlaylistId == playlistId);
            if (playlist == null)
            {
                return;
            }
            context.Playlists.Remove(playlist);
            await context.SaveChangesAsync();
        }

        public async Task UpdatePlaylistTracking(string playlistId, bool newValueForIsBeingTracked)
        {
            using var context = _factory.CreateDbContext();
            var playlist = context.Playlists.SingleOrDefault(playlist => playlist.PlaylistId == playlistId);
            if (playlist == null)
            {
                return;
            }

            playlist.IsBeingTracked = newValueForIsBeingTracked;
            await context.SaveChangesAsync();
        }
    }
}
