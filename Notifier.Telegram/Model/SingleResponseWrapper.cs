using Newtonsoft.Json;

namespace Notifier.Telegram.Model
{
    public class SingleResponseWrapper<T>
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public required T Item { get; set; }
    }
}
