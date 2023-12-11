using Microsoft.Extensions.Logging;
using Notifier.Logic.Services;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Helpers;

namespace Notifier.Telegram.Implementation.Command
{
    internal class PlaylistsCommandHandler : CommandHandler<PlaylistsCommandHandler>
    {
        public static string Command => "/playlists";
        public static string CommandDescription => "get playlists available for tracking";

        private readonly PlaylistsService _playlistsService;

        public PlaylistsCommandHandler(ITelegramRestClient telegramClient, PlaylistsService playlistsService,  ILogger<PlaylistsCommandHandler> logger) 
            : base(telegramClient, logger)
        {
            _playlistsService = playlistsService;
        }

        public override async Task Handle(long chatId, string parameters)
        {
            _logger.LogInformation("{command} command received", Command);

            var playlists = await _playlistsService.GetPlaylists();

            var replyMessageBuilder = TelegramResponseMessageBuilder
               .New
               .WithChatId(chatId)
               .Notify(false)
               .AddLine("These are all available playlists:", bold: true, underlined: true);

            foreach(var playlist in playlists)
            {
                replyMessageBuilder
                    .AddText("▶ ")
                    .AddLink(playlist.Title, playlist.Url)
                    .AddText(" (by ")
                    .AddLink(playlist.Owner.DisplayName, playlist.Owner.Url)
                    .AddLine(");");
            }

            await SendMessage(replyMessageBuilder.BuildMessage());
            _logger.LogInformation("{command} command handled", Command);
        }
    }
}
