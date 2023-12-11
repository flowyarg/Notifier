using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifier.DataAccess.Settings;

namespace Notifier.DataAccess.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var dataAccessSettings = configuration
                 .GetRequiredSection(nameof(NotifierDataAccessSettings))
                 .Get<NotifierDataAccessSettings>()
                 ?? throw new Exception($"{nameof(NotifierDataAccessSettings)} not found in application configuration");

            services.AddDbContextFactory<NotifierDbContext>(options =>
            {
                options.UseNpgsql(dataAccessSettings.ConnectionString);
            });
        }
    }
}
