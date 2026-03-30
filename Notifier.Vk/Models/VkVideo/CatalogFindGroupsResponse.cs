namespace Notifier.Vk.Models.VkVideo;

using Newtonsoft.Json;

public class CatalogFindGroupsResponse
{
    [JsonProperty("groups")]
    public GroupInfo[] Groups { get; set; }
}