using Notifier.Telegram.Model;
using Notifier.Telegram.Model.Incoming;
using Notifier.Telegram.Model.Outgoing;
using System.Text;

namespace Notifier.Telegram.Helpers
{
    internal class TelegramResponseMessageBuilder
    {
        private readonly StringBuilder _messageBodyBuilder;
        private readonly List<MessageEntity> _messageEntities;
        private long? _chatId;
        private bool _shouldNotify = true;

        private TelegramResponseMessageBuilder()
        {
            _messageBodyBuilder = new StringBuilder();
            _messageEntities = [];
        }

        public static TelegramResponseMessageBuilder New => new();

        public TelegramResponseMessageBuilder WithChatId(long chatId)
        {
            _chatId = chatId;
            return this;
        }

        public TelegramResponseMessageBuilder AddText(string? text = null, bool bold = false, bool italic = false, bool underlined = false, bool strikethrough = false)
        {
            if (text == null)
            {
                return this;
            }

            var startIndex = _messageBodyBuilder.Length;
            var length = text.Length;
            _messageBodyBuilder.Append(text);

            if (bold)
            {
                AddEntity(MessageEntityType.BoldText);
            }
            if (italic)
            {
                AddEntity(MessageEntityType.ItalicText);
            }
            if (underlined)
            {
                AddEntity(MessageEntityType.UnderlinedText);
            }
            if (strikethrough)
            {
                AddEntity(MessageEntityType.StrikethroughText);
            }

            void AddEntity(MessageEntityType type)
            {
                _messageEntities.Add(new MessageEntity
                {
                    Type = type,
                    Offset = startIndex,
                    Length = length,
                });
            }

            return this;
        }

        public TelegramResponseMessageBuilder AddLine(string? text = null, bool bold = false, bool italic = false, bool underlined = false, bool strikethrough = false)
        {
            if (text != null)
            {
                AddText(text, bold, italic, underlined, strikethrough);
            }
            _messageBodyBuilder.AppendLine(null);
            return this;
        }

        public TelegramResponseMessageBuilder AddCommand(string command, string? description = null)
        {
            var startIndex = _messageBodyBuilder.Length;
            var length = command.Length;

            _messageBodyBuilder.Append(command);
            if (description != null)
            {
                _messageBodyBuilder.Append($" - {description}");
            }

            _messageEntities.Add(new MessageEntity
            {
                Type = MessageEntityType.BotCommand,
                Offset = startIndex,
                Length = length
            });

            return this;
        }

        public TelegramResponseMessageBuilder AddCommandLine(string command, string? description = null)
        {
            AddCommand(command, description);
            _messageBodyBuilder.AppendLine(null);
            return this;
        }

        public TelegramResponseMessageBuilder AddLink(string text, string url)
        {
            var startIndex = _messageBodyBuilder.Length;
            var length = text.Length;
            _messageBodyBuilder.Append(text);

            _messageEntities.Add(new MessageEntity
            {
                Type = MessageEntityType.TextLink,
                Url = url,
                Offset = startIndex,
                Length = length
            });
            return this;
        }

        public TelegramResponseMessageBuilder AddLinkLine(string text, string url)
        {
            AddLink(text, url);
            _messageBodyBuilder.AppendLine(null);
            return this;
        }

        public TelegramResponseMessageBuilder Notify(bool shouldNotify = true)
        {
            _shouldNotify = shouldNotify;
            return this;
        }

        public SendMessageRequest BuildMessage()
        {
            if (_chatId == null)
            {
                throw new Exception("No chat id was specified");
            }
            if (_messageBodyBuilder.Length == 0)
            {
                throw new Exception("No message body was specified");
            }

            return new SendMessageRequest
            {
                ChatId = _chatId.Value,
                Text = _messageBodyBuilder.ToString(),
                Entities = _messageEntities,
                DisableWebPagePreview = true,
                DisableNotification = !_shouldNotify,
            };
        }
    }
}
