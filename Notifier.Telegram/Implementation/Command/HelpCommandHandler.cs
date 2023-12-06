using Microsoft.Extensions.Logging;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;

namespace Notifier.Telegram.Implementation.Command
{
    internal class HelpCommandHandler : CommandHandler<HelpCommandHandler>
    {
        public static string Command => "/help";
        public static string CommandDescription => "show help message";

        public HelpCommandHandler(ITelegramRestClient telegramClient, ILogger<HelpCommandHandler> logger) 
            : base(telegramClient, logger)
        { }

        public override async Task Handle(long chatId, string parameters)
        {
            var replyMessage = TelegramResponseMessageBuilder
                 .New
                 .WithChatId(chatId)
                 .AddLine("I can notify you about new videos in playlists you are interested in (currently supporting Vk playlists only).")
                 .AddLine()
                 .AddLine("You can control me by sending these commands:")
                 .AddLine()
                 .AddLine("Help commands:", bold: true, underlined: true)
                 .AddCommandLine("/start", "start bot and show help message")
                 .AddCommandLine("/help", "show help message")
                 .AddLine()
                 .AddLine("Playlist commands:", bold: true, underlined: true)
                 .AddCommandLine("/playlists", "get playlists available for tracking")
                 .AddCommandLine("/subscribe", "subscribe to notifications in certain playlist")
                 .AddCommandLine("/unsubscribe", "unsubscribe from notifications in certain playlist")
                 .AddCommandLine("/my_subscriptions", "list all playlists i am subscribed to")
                 .AddCommandLine("/subscribe_all", "subscribe to notifications in all playlists")
                 .AddCommandLine("/unsubscribe_all", "unsubscribe from notifications in all playlists")
                 .AddLine()
                 .AddText("I am still a work in progress =^_^=", italic: true)
                 .Notify(false)
                 .BuildMessage();

            await SendMessage(replyMessage);
            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
