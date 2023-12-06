using Microsoft.EntityFrameworkCore;
using Notifier.DataAccess;
using Notifier.Logic.Models;

namespace Notifier.Logic.Services
{
    public class OwnersService
    {
        private readonly IDbContextFactory<NotifierDbContext> _factory;

        public OwnersService(IDbContextFactory<NotifierDbContext> factory)
        {
            _factory = factory;
        }

        public async Task AddUser(string id, string firstName, string lastName, string url)
        {
            using var context = _factory.CreateDbContext();
            context.Users.Add(new DataAccess.Model.User { OwnerId = id, FirstName = firstName, LastName = lastName, Url = url });
            await context.SaveChangesAsync();
        }

        public async Task AddGroup(string id, string name, string url)
        {
            using var context = _factory.CreateDbContext();
            context.Groups.Add(new DataAccess.Model.Group { OwnerId = id, Name = name, Url = url });
            await context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Owner>> GetOwners()
        {
            using var context = _factory.CreateDbContext();

            var users = await GetUsers(context);
            var groups = await GetGroups(context);

            return users
                .Cast<Owner>()
                .Union(groups.Cast<Owner>())
                .ToArray();
        }

        public async Task<IReadOnlyCollection<Owner>> GetOwners(IReadOnlyCollection<string> ownerIds)
        {
            var idsToSearch = ownerIds.ToHashSet();
            using var context = _factory.CreateDbContext();
            var owners = await (from owner in context.Owners
                          join user in context.Users on owner.Id equals user.Id into userJoin
                          from u in userJoin.DefaultIfEmpty()
                          join @group in context.Groups on owner.Id equals @group.Id into groupJoin
                          from g in groupJoin.DefaultIfEmpty()
                          where idsToSearch.Contains(owner.OwnerId)
                          select new
                          {
                              Id = owner.OwnerId,
                              Url = owner.Url,
                              GroupName = g == null ? null : g.Name,
                              UserFirstName = u == null ? null : u.FirstName,
                              UserLastName = u == null ? null : u.LastName
                          }).AsNoTracking().ToArrayAsync();

            var users = owners
                .Where(owner => owner.UserFirstName != null && owner.UserLastName != null)
                .Select(owner => new VkUser { Id = owner.Id, FirstName = owner.UserFirstName, LastName = owner.UserLastName, Url = owner.Url });

            var groups = owners
                .Where(owner => owner.GroupName != null)
                .Select(owner => new VkGroup { Id = owner.Id, Name = owner.GroupName, Url = owner.Url });

            return users
               .Cast<Owner>()
               .Union(groups.Cast<Owner>())
               .ToArray();
        }

        public async Task DeleteOwner(string ownerId)
        {
            using var context = _factory.CreateDbContext();
            var owner = context.Owners.SingleOrDefault(owner => owner.OwnerId == ownerId);
            if (owner == null)
            {
                return;
            }
            context.Owners.Remove(owner);
            await context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<VkGroup>> GetGroups()
        {
            using var context = _factory.CreateDbContext();
            return await GetGroups(context);
        }

        public async Task<IReadOnlyCollection<VkUser>> GetUsers()
        {
            using var context = _factory.CreateDbContext();
            return await GetUsers(context);
        }

        private static async Task<IReadOnlyCollection<VkGroup>> GetGroups(NotifierDbContext context)
        {
            var groups = await context
                .Groups
                .AsNoTracking()
                .ToListAsync();
            return groups.Select(group => new VkGroup
            {
                Id = group.OwnerId,
                Name = group.Name,
                Url = group.Url,
            }).ToArray();
        }

        private static async Task<IReadOnlyCollection<VkUser>> GetUsers(NotifierDbContext context)
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users.Select(user => new VkUser
            {
                Id = user.OwnerId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Url = user.Url,
            }).ToArray();
        }
    }
}
