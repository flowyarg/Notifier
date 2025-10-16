namespace Notifier.Vk.Models.VkVideo;

using Newtonsoft.Json;

public class CatalogGetVideoResponse
{
    [JsonProperty("albums")]
    public VideoAlbumInfo[] Albums { get; set; }
}