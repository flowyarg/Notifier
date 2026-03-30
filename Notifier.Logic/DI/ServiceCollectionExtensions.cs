using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifier.Logic.Services;

namespace Notifier.Logic.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<OwnersService>();
            services.AddTransient<PlaylistsService>();
            services.AddTransient<VideosService>();
            services.AddTransient<PlaylistSubscriptionService>();
        }
    }
}
