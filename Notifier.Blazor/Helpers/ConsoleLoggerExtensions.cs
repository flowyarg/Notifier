using System;

namespace Notifier.Blazor.Helpers
{
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddCustomFormatter(
            this ILoggingBuilder builder,
            Action<CustomColorOptions> configure) =>
            builder.AddConsole(options => options.FormatterName = "customName")
                .AddConsoleFormatter<CustomColorFormatter, CustomColorOptions>(configure);
    }
}
