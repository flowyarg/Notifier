using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifier.Matrix.Contract;
using Notifier.Matrix.Implementation;
using Notifier.Matrix.Services;

namespace Notifier.Matrix.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMatrixApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMatrixRestClient, MatrixRestClient>();
            services.AddSingleton<MatrixRestClientWithNoAuthorization>();
            services.AddSingleton<MatrixRestClientWithAuthorization>();
            services.AddSingleton<MadgeburgMatrixRestClientWithNoAuthorization>();
            
            services.AddSingleton<SimpleMatrixService>();
            services.AddSingleton<MatrixCredentialsService>();
            AddCommandHandlers(services);
            AddUpdateHandlers(services);
        }

        private static void AddCommandHandlers(IServiceCollection services)
        {
            // var handlerTypes = typeof(ICommandHandler).Assembly
            //     .GetTypes()
            //     .Where(type => typeof(ICommandHandler).IsAssignableFrom(type))
            //     .Where(type => type.IsClass)
            //     .Where(type => !type.IsAbstract)
            //     .ToDictionary(handlerType => (string)handlerType.GetProperty(nameof(ICommandHandler.Command))!.GetValue(null)!, handlerType => handlerType);
            //
            // foreach(var type in handlerTypes.Values)
            // { 
            //     services.AddTransient(type);
            // }
            //
            // services.AddSingleton<CommandHandlerResolver>(provider => command =>
            // {
            //     var hasHandler = handlerTypes.TryGetValue(command, out var handlerType);
            //     if (!hasHandler)
            //     {
            //         throw new KeyNotFoundException($"No handler found for command: {command}");
            //     }
            //     return (ICommandHandler)provider.GetRequiredService(handlerType!);
            // });
        }

        private static void AddUpdateHandlers(IServiceCollection services)
        {
            // services.AddTransient<GenericMessageUpdateHandler>();
            // services.AddTransient<BotCommandReplyUpdateHandler>();
            // services.AddTransient<BotCommandUpdateHandler>();
            // services.AddTransient<CallbackQueryUpdateHandler>();
            //
            // services.AddTransient<IUpdateHandler>(provider =>
            // {
            //     var callbackHandler = provider.GetRequiredService<CallbackQueryUpdateHandler>();
            //     var botCommandReplyHandler = provider.GetRequiredService<BotCommandReplyUpdateHandler>();
            //     var botCommandHandler = provider.GetRequiredService<BotCommandUpdateHandler>();
            //     var genericMessageHandler = provider.GetRequiredService<GenericMessageUpdateHandler>();
            //
            //     callbackHandler.NextHandler = botCommandHandler;
            //     botCommandHandler.NextHandler = botCommandReplyHandler;
            //     botCommandReplyHandler.NextHandler = genericMessageHandler;
            //
            //     return callbackHandler;
            // });
        }
    }
}
