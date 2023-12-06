using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Model;
using Notifier.Telegram.Model.Outgoing;
using Notifier.Telegram.Settings;
using RestSharp;
using In = Notifier.Telegram.Model.Incoming;
using Out = Notifier.Telegram.Model.Outgoing;

namespace Notifier.Telegram.Implementation
{
    internal class TelegramRestClient : RestClient, ITelegramRestClient
    {
        private readonly JsonSerializerSettings _serializationSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public TelegramRestClient(IOptions<TelegramApiSettings> apiSettings)
          : base(new RestClientOptions($"https://api.telegram.org/bot{apiSettings.Value.AccessToken}"))
        { }

        public async Task<IReadOnlyCollection<In.Update>> GetUpdates(int timeout = 0, int limit = 100, int? offset = null)
        {
            var request = new RestRequest("getUpdates")
                .AddParameter("limit", limit)
                .AddParameter("timeout", timeout)
                .AddParameter("allowed_updates", """
                ["message", "callback_query"]
                """);

            if (offset != null)
            {
                request.AddParameter("offset", offset.Value);
            }

            var response = await HandleRequest<CollectionResponseWrapper<In.Update>>(request);
            return response!.Items;
        }

        public async Task<bool> SendMessage<T>(T message)
            where T : SendRequest
        {
            var body = JsonConvert.SerializeObject(message, _serializationSettings);

            var request = new RestRequest(GetMethodName<T>(), method: Method.Post)
                .AddJsonBody(body, forceSerialize: false);

            var response = await HandleRequest<SingleResponseWrapper<In.Message>>(request);

            return response!.Ok;
        }

        private static string GetMethodName<T>()
            where T : SendRequest
        {
            if (typeof(T) == typeof(Out.SendMessageRequest))
            {
                return "sendMessage";
            }
            if (typeof(T) == typeof(Out.SendPhotoRequest))
            {
                return "sendPhoto";
            }
            throw new ArgumentException($"Unexpected message type: {typeof(T)}");
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
