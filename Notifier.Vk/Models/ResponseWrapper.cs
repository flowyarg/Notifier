﻿using Newtonsoft.Json;

namespace Notifier.Vk.Models
{
    public class ResponseWrapper<TResponse>
    {
        [JsonProperty("response")]
        public TResponse? Response { get; set; }
    }
}
