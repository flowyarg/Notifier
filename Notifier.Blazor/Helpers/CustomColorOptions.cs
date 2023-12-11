using Microsoft.Extensions.Logging.Console;

namespace Notifier.Blazor.Helpers
{
    public class CustomColorOptions : SimpleConsoleFormatterOptions
    {
        public string? CustomPrefix { get; set; }
    }
}
