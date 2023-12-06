using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal class GenericMessageUpdateHandler : UpdateHandler
    {
        private readonly ITelegramRestClient _telegramClient;

        public GenericMessageUpdateHandler(ITelegramRestClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        protected override bool CanHandle(Update update)
        {
            if (update.Message == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(update.Message.Text))
            {
                return false;
            }

            return true;
        }

        protected override async Task<bool> HandleInternal(Update update)
        {
            var replyMessage = TelegramResponseMessageBuilder
              .New
              .WithChatId(update!.Message!.Chat!.Id)
              .AddLine("Huh?", bold: true, italic: true)
              .AddLine()
              .AddLine("／人◕ __ ◕人＼")
              .AddLine()
              .AddLine("I can't get what you're saying!", italic: true)
              .AddLine()
              .AddText("༼ つ ◕_◕ ༽つ some ")
              .AddLine("commands", bold: true, italic: true, underlined: true)
              .AddLine()
              .AddText("Or perhaps you need some ")
              .AddCommand("/help")
              .AddLine("?")
              .BuildMessage();

            return await _telegramClient.SendMessage(replyMessage);
        }
    }
}
