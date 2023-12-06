using Notifier.Telegram.Model.Incoming;

namespace Notifier.Telegram.Services.Handlers
{
    internal abstract class UpdateHandler : IUpdateHandler
    {
        public IUpdateHandler? NextHandler { get; set; }

        public async Task<bool> Handle(Update update)
        {
            if (CanHandle(update))
            {
                var wasHandled = await HandleInternal(update);

                if (wasHandled)
                {
                    return true;
                }
            }

            if (NextHandler == null)
            {
                return false;
            }

            return await NextHandler.Handle(update);
        }

        protected abstract bool CanHandle(Update update);
        protected abstract Task<bool> HandleInternal(Update update);
    }
}
