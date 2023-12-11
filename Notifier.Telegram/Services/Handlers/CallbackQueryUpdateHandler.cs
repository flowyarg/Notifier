using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal class CallbackQueryUpdateHandler : UpdateHandler
    {
        public CallbackQueryUpdateHandler()
        {}

        protected override bool CanHandle(Update update)
        {
            if (update.CallbackQuery == null)
            {
                return false;
            }

            if (update.Message != null)
            {
                return false;
            }

            return true;
        }

        protected override async Task<bool> HandleInternal(Update update)
        {
            return await Task.FromResult(true);
        }
    }
}
