using Notifier.Vk.Contract;
using Notifier.Vk.Implementation;

namespace Notifier.Vk.DI
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddVkApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IVkRestClientBuilder, VkRestClientBuilder>();
            services.AddTransient<IVkAuthenticationRestClientBuilder, VkAuthenticationRestClientBuilder>();
            services.AddTransient<IVkVideoRestClient, VkVideoRestClient>();
        }
    }
}
