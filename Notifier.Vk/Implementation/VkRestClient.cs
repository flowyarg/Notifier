using Newtonsoft.Json;
using Notifier.Vk.Contract;
using Notifier.Vk.Models;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace Notifier.Vk.Implementation
{
    internal class VkRestClient : RestClient, IVkRestClient
    {
        private const string _apiEndpoint = "https://api.vk.com/method";
        private const string _apiVersion = "5.154";

        private const string _vkPlaylistBaseUrlFormat = "https://vk.com/video/playlist/{0}_{1}";
        private const string _vkBaseUrlFormat = "https://vk.com/{0}";

        public VkRestClient(string accessToken)
            : base(
                  new RestClientOptions(_apiEndpoint)
                  {
                      Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer")
                  })
        {
            DefaultParameters.AddParameter(new QueryParameter("v", _apiVersion));
        }

        public async Task<IReadOnlyCollection<GroupInfo>> FindGroups(string searchString, int count = 10)
        {
            var request = new RestRequest("groups.search")
                .AddParameter("q", searchString)
                .AddParameter("sort", "0")
                .AddParameter("count", count);

            var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<GroupInfo>>>(request);

            foreach(var group in responseData!.Response!.Items)
            {
                group.Url = string.Format(_vkBaseUrlFormat, group.ScreenName);
            }

            return responseData!.Response!.Items ?? [];
        }

        public async Task<IReadOnlyCollection<UserInfo>> FindUsers(string searchString, int count = 10)
        {
            var request = new RestRequest("users.search")
                .AddParameter("q", searchString)
                .AddParameter("sort", "0")
                .AddParameter("count", count)
                .AddParameter("fields", "domain");

            var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<UserInfo>>>(request);

            foreach(var user in responseData!.Response!.Items)
            {
                user.Url = string.Format(_vkBaseUrlFormat, user.Domain);
            }

            return responseData!.Response!.Items ?? [];
        }

        public async Task<IReadOnlyCollection<VideoAlbumInfo>> FindVideoAlbums(string ownerId, VkOwnerType ownerType, int count = 50)
        {
            if (ownerType == VkOwnerType.Group)
            {
                ownerId = "-" + ownerId;
            }

            var request = new RestRequest("video.getAlbums")
                .AddParameter("owner_id", ownerId)
                .AddParameter("need_system", 1)
                .AddParameter("count", count);

            var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<VideoAlbumInfo>>>(request);

            foreach(var playlist in  responseData!.Response!.Items)
            {
                playlist.Url = string.Format(_vkPlaylistBaseUrlFormat, ownerId, playlist.Id);
            }

            return responseData!.Response?.Items ?? [];
        }

        public async Task<IReadOnlyCollection<UserInfo>> GetFriends(string userId)
        {
            var request = new RestRequest("friends.get")
               .AddParameter("user_id", userId)
               .AddParameter("fields", "bdate, city, last_seen");

            var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<UserInfo>>>(request);

            return responseData!.Response?.Items ?? [];
        }

        public async IAsyncEnumerable<VideoInfo> GetVideos(string ownerId, VkOwnerType ownerType, string albumId)
        {
            if (ownerType == VkOwnerType.Group)
            {
                ownerId = "-" + ownerId;
            }
            const int count = 50;
            var currentOffset = 0;

            for (; ; )
            {
                var request = new RestRequest("video.get")
                    .AddParameter("owner_id", ownerId)
                    .AddParameter("album_id", albumId)
                    .AddParameter("offset", currentOffset)
                    .AddParameter("count", count)
                    .AddParameter("sort_album", 1);

                currentOffset += count;

                var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<VideoInfo>>>(request);

                if (responseData!.Response?.Items == null)
                {
                    yield break;
                }

                foreach (var video in responseData!.Response!.Items)
                {
                    yield return video;
                }

                if (currentOffset >= responseData.Response.Count)
                {
                    break;
                }
            }
        }

        private async Task<TResponse?> HandleRequest<TResponse>(RestRequest request)
        {
            try
            {
                var response = await this.GetAsync(request);

                return JsonConvert.DeserializeObject<TResponse>(response.Content!);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }
    }
}
