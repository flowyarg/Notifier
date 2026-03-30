namespace Notifier.Vk.Models.VkVideo;

using Newtonsoft.Json;

public class VkVideoWrapper
{
    [JsonProperty("playlist_position")]
    public int PlaylistPosition { get; set; }
    
    [JsonProperty("video")]
    public VideoInfo VideoInfo { get; set; }
}