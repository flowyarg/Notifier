namespace Notifier.Vk.Implementation;

using Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.VkVideo;
using Newtonsoft.Json;
using RestSharp;

public class VkVideoRestClient : RestClient, IVkVideoRestClient
{
    private const string _apiEndpoint = "https://api.vkvideo.ru/method";
    private const string _vkVideoPlaylistBaseUrlFormat = "https://vkvideo.ru/playlist/{0}_{1}";
    
    private const string _vkVideoBaseUrlFormat = "https://vkvideo.ru/@{0}";
    
    private readonly ILogger<VkVideoRestClient> _logger;
    private readonly string _accessToken;
    
    public VkVideoRestClient(ILogger<VkVideoRestClient> logger, IOptions<VkVideoApiCredentials> apiOptions)
        : base(new RestClientOptions(_apiEndpoint))
    {
        DefaultParameters.AddParameter(new QueryParameter("v", apiOptions.Value.ApiVersion));
        DefaultParameters.AddParameter(new QueryParameter("client_id", apiOptions.Value.ClientId));
        
        _logger = logger;
        _accessToken = apiOptions.Value.AccessToken;
    }
    
    public async IAsyncEnumerable<VideoInfo> GetVideos(string ownerId, VkOwnerType ownerType, string albumId)
    {
        if (ownerType == VkOwnerType.Group)
        {
            ownerId = "-" + ownerId;
        }
        const int count = 25;
        var currentOffset = 0;
        

        for (; ; )
        {
            var request = new RestRequest("video.getFromAlbum", Method.Post)
                .AddParameter("album_id", albumId)
                .AddParameter("count", count)
                .AddParameter("extended", 1)
                .AddParameter("fields", "photo_50,verified")
                .AddParameter("offset", currentOffset)
                .AddParameter("owner_id", ownerId)
                .AddParameter("sort_album", 1);
            
            var responseData = await HandleRequest<ResponseWrapper<CollectionResponseWrapper<VkVideoWrapper>>>(request);

            if (responseData!.Error is not null)
            {
                _logger.LogWarning("Request to {Url} resulted in error: Code: {ErrorCode}, Message: {ErrorMessage}", request.Resource, responseData.Error.ErrorCode, responseData.Error.ErrorMessage);
                if (responseData!.Error.ErrorCode == 6) // Too many requests per second
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    continue;
                }
            }

            currentOffset += count;

            if (responseData!.Response?.Items == null)
            {
                yield break;
            }

            foreach (var video in responseData!.Response!.Items)
            {
                yield return video.VideoInfo;
            }

            if (currentOffset >= responseData.Response.Count)
            {
                yield break;
            }
        }
    }
    
    public async Task<IReadOnlyCollection<VideoAlbumInfo>> FindVideoAlbums(string ownerId, VkOwnerType ownerType, int count = 50)
    {
        if (ownerType == VkOwnerType.Group)
        {
            ownerId = "-" + ownerId;
        }

        var request = new RestRequest("catalog.getVideo", Method.Post)
            .AddParameter("need_blocks", "1")
            .AddParameter("owner_id", ownerId);

        var responseData = await HandleRequest<ResponseWrapper<CatalogGetVideoResponse>>(request);

        foreach(var playlist in  responseData!.Response!.Albums)
        {
            playlist.Url = string.Format(_vkVideoPlaylistBaseUrlFormat, ownerId, playlist.Id);
        }

        return responseData!.Response?.Albums ?? [];
    }
    
    public async Task<IReadOnlyCollection<GroupInfo>> FindGroups(string searchString)
    {
        var request = new RestRequest("catalog.getVideoSearchWeb2", Method.Post)
            .AddParameter("screen_ref", "search_video_service")
            .AddParameter("q", searchString)
            .AddParameter("input_method", "keyboard_search_button");

        var responseData = await HandleRequest<ResponseWrapper<CatalogFindGroupsResponse>>(request);

        foreach(var group in responseData!.Response!.Groups)
        {
            group.Url = string.Format(_vkVideoBaseUrlFormat, group.ScreenName);
        }

        return responseData!.Response!.Groups;
    }
    
    private async Task<TResponse?> HandleRequest<TResponse>(RestRequest request)
    {
        try
        {
            request.AddParameter("access_token", _accessToken);
            var response = await this.PostAsync(request);
            _logger.LogInformation("Request to {Url} executed", request.Resource);
            return JsonConvert.DeserializeObject<TResponse>(response.Content!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Request to {Url} failed", request.Resource);
            throw;
        }
    }
}