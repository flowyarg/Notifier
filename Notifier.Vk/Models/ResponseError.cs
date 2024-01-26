using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class ResponseError
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string? ErrorMessage { get; set; }

        [JsonProperty("request_params")]
        public IReadOnlyCollection<RequestParameter>? RequestParameters { get; set; }

        public class RequestParameter
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
