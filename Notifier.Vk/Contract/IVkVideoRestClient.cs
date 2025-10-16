namespace Notifier.Vk.Contract;

using Models;

public interface IVkVideoRestClient
{
    IAsyncEnumerable<VideoInfo> GetVideos(string ownerId, VkOwnerType ownerType, string albumId);
    Task<IReadOnlyCollection<VideoAlbumInfo>> FindVideoAlbums(string ownerId, VkOwnerType ownerType, int count = 50);
    Task<IReadOnlyCollection<GroupInfo>> FindGroups(string searchString);
}