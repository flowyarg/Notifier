using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifier.Vk.Contract;
using Notifier.Vk.Implementation;

namespace Notifier.Vk.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVkApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IVkRestClientBuilder, VkRestClientBuilder>();
            services.AddTransient<IVkAuthenticationRestClientBuilder, VkAuthenticationRestClientBuilder>();
        }
    }
}
