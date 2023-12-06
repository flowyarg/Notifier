using Newtonsoft.Json;

namespace Notifier.Telegram.Model
{
    //{"ok":true,"result":[]}
    public class CollectionResponseWrapper<T>
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public required IReadOnlyCollection<T> Items { get; set; }
    }
}
