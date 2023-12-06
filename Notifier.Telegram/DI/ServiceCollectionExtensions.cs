using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifier.Telegram.Contract;
using Notifier.Telegram.Contract.Command;
using Notifier.Telegram.Helpers;
using Notifier.Telegram.Implementation;
using Notifier.Telegram.Services;
using Notifier.Telegram.Services.Handlers;

namespace Notifier.Telegram.DI
{
    public delegate ICommandHandler CommandHandlerResolver(string command);
    public static class ServiceCollectionExtensions
    {
        public static void AddTelegramApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITelegramRestClient, TelegramRestClient>();
            services.AddSingleton<CommandContextStore>();

            services.AddTransient<ITelegramBotUpdatesService, TelegramBotUpdatesService>();
            services.AddTransient<IPlaylistNotificationService, PlaylistNotificationService>();

            AddCommandHandlers(services);
            AddUpdateHandlers(services);
        }

        private static void AddCommandHandlers(IServiceCollection services)
        {
            var handlerTypes = typeof(ICommandHandler).Assembly
                .GetTypes()
                .Where(type => typeof(ICommandHandler).IsAssignableFrom(type))
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .ToDictionary(handlerType => handlerType.GetProperty(nameof(ICommandHandler.Command))!.GetValue(null)!, handlerType => handlerType);

            foreach(var type in handlerTypes.Values)
            { 
                services.AddTransient(type);
            }
            
            services.AddSingleton<CommandHandlerResolver>(provider => command =>
            {
                var hasHandler = handlerTypes.TryGetValue(command, out var handlerType);
                if (!hasHandler)
                {
                    throw new KeyNotFoundException($"No handler found for command: {command}");
                }
                return (ICommandHandler)provider.GetRequiredService(handlerType!);
            });
        }

        private static void AddUpdateHandlers(IServiceCollection services)
        {
            services.AddTransient<GenericMessageUpdateHandler>();
            services.AddTransient<BotCommandReplyUpdateHandler>();
            services.AddTransient<BotCommandUpdateHandler>();
            services.AddTransient<CallbackQueryUpdateHandler>();

            services.AddTransient<IUpdateHandler>(provider =>
            {
                var callbackHandler = provider.GetRequiredService<CallbackQueryUpdateHandler>();
                var botCommandReplyHandler = provider.GetRequiredService<BotCommandReplyUpdateHandler>();
                var botCommandHandler = provider.GetRequiredService<BotCommandUpdateHandler>();
                var genericMessageHandler = provider.GetRequiredService<GenericMessageUpdateHandler>();

                callbackHandler.NextHandler = botCommandHandler;
                botCommandHandler.NextHandler = botCommandReplyHandler;
                botCommandReplyHandler.NextHandler = genericMessageHandler;

                return callbackHandler;
            });
        }
    }
}
