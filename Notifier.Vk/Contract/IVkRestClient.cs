using Notifier.Vk.Models;

namespace Notifier.Vk.Contract
{
    public interface IVkRestClient : IDisposable
    {
        Task<IReadOnlyCollection<GroupInfo>> FindGroups(string searchString, int count = 10);
        Task<IReadOnlyCollection<UserInfo>> FindUsers(string searchString, int count = 10);
        Task<IReadOnlyCollection<VideoAlbumInfo>> FindVideoAlbums(string ownerId, VkOwnerType ownerType, int count = 50);
        IAsyncEnumerable<VideoInfo> GetVideos(string ownerId, VkOwnerType ownerType, string albumId);
        Task<IReadOnlyCollection<UserInfo>> GetFriends(string userId);
    }
}
