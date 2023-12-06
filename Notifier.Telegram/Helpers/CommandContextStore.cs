namespace Notifier.Telegram.Helpers
{
    internal class CommandContextStore
    {
        private readonly Dictionary<long, string?> _commandContexts = [];

        public void StartCommandContext(long chatId, string commandName)
        {
            if (!_commandContexts.TryAdd(chatId, commandName))
            {
                _commandContexts[chatId] = commandName;
            }
        }

        public string? GetCommandContext(long chatId)
        {
            return _commandContexts.TryGetValue(chatId, out var commandContext) ? commandContext : null;
        }

        public void StopCommandContext(long chatId)
        {
            if (!_commandContexts.TryGetValue(chatId, out var commandContext) || commandContext == null)
            {
                throw new InvalidOperationException($"Context was not started for chatId: {chatId}");
            }
            _commandContexts.Remove(chatId);
        }
    }
}
