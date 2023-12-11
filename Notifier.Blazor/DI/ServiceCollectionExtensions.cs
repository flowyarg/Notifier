using Coravel;
using Microsoft.EntityFrameworkCore;
using Notifier.Blazor.Helpers;
using Notifier.Blazor.Jobs;
using Notifier.Blazor.Settings;
using Notifier.DataAccess;
using Notifier.Logic.Services;
using Notifier.Logic.Services.Security;
using Notifier.Telegram.Settings;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Diagnostics;

namespace Notifier.Blazor.DI
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var notifierSettings = configuration
                .GetRequiredSection(nameof(NotifierSettings))
                .Get<NotifierSettings>()
                ?? throw new Exception($"{nameof(NotifierSettings)} not found in application configuration");

            services.AddSingleton(provider => new AccessTokenService(
                provider.GetRequiredService<IDbContextFactory<NotifierDbContext>>(),
                provider.GetRequiredService<AESCrypto>(),
                notifierSettings.EncryptionKey));

            services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

            services.AddTransient<TokenRefreshmentJob>();
            services.AddTransient<SyncPlaylistsJob>();
            services.AddTransient<PlaylistNotificationJob>();
            services.AddTransient<TelegramBotRunnerJob>();

            services.AddTransient<IWebDriver>(_ => CreateDockerChromeDriver());
            //services.AddTransient<IWebDriver>(_ => CreateLocalChromeDriver());
        }

        public static void ConfigureScheduler(this IServiceProvider provider)
        {
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<TokenRefreshmentJob>()
                    .EveryTenMinutes()
                    .RunOnceAtStart()
                    .PreventOverlapping(nameof(TokenRefreshmentJob));

                scheduler.Schedule<SyncPlaylistsJob>()
                    .Hourly()
                    .RunOnceAtStart()
                    .PreventOverlapping(nameof(SyncPlaylistsJob));

                scheduler.Schedule<TelegramBotRunnerJob>()
                    .EverySeconds(15)
                    .PreventOverlapping(nameof(TelegramBotRunnerJob));
            });
        }

        public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NotifierSettings>(configuration.GetRequiredSection(nameof(NotifierSettings)));
            services.Configure<VkApiSettings>(configuration.GetRequiredSection(nameof(VkApiSettings)));
            services.Configure<TelegramApiSettings>(configuration.GetRequiredSection(nameof(TelegramApiSettings)));
        }

        private static RemoteWebDriver CreateDockerChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--lang=en");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--window-size=1920,1080");
            chromeOptions.AddArgument("--enable-javascript");

             var seleniumUrl = $"http://{(Debugger.IsAttached ? "host.docker.internal:4445" : "selenium.standalone:4444")}/wd/hub";
            //var seleniumUrl = "http://localhost:4445/wd/hub";
            return new RemoteWebDriver(new Uri(seleniumUrl), chromeOptions);
        }

        private static ChromeDriver CreateLocalChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            // chromeOptions.AddExtension("S:\\chromedriver\\4.46.2_0.crx");
            chromeOptions.AddArgument("--lang=en");
            chromeOptions.AddArgument("--window-size=1600,900");
            chromeOptions.AddArgument("--enable-javascript");
            var driver = new ChromeDriver("S:\\chromedriver", chromeOptions);
            return driver;
        }
    }
}
