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
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Diagnostics;
using System.Reflection;

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

            services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));

            services.AddTransient<TokenRefreshmentJob>();
            services.AddTransient<SyncPlaylistsJob>();
            services.AddTransient<PlaylistNotificationJob>();
            services.AddTransient<TelegramBotRunnerJob>();

            services.AddTransient<IWebDriver>(_ => CreateDockerChromeDriver());
            //services.AddTransient<IWebDriver>(_ => CreateLocalChromeDriver());
        }

        public static void AddCustomOpenTelemetry(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .WithMetrics(x =>
                {
                    x.AddPrometheusExporter();
                    x.AddMeter(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "System.Net.Http",
                        "Notifier.Blazor");
                    
                    x.AddView("request-duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1]
                    });
                });

            services.AddMetrics();
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

        public static void ConfigureCustomSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            hostBuilder.UseSerilog(logger);
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

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            var elasticUri = configuration["ElasticConfiguration:Uri"]!;
            return new(new Uri(elasticUri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
