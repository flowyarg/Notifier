using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class CollectionResponseWrapper<T>
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("items")]
        public IReadOnlyCollection<T> Items { get; set; }
    }
}
