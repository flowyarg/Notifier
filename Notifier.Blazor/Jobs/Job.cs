using Coravel.Invocable;
using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Jobs
{
    internal abstract class Job<T> : IInvocable
    {
        protected readonly ILogger<T> _logger;
        protected readonly IMeterFactory _meterFactory;

        public Job(ILogger<T> logger, IMeterFactory meterFactory)
        {
            _logger = logger;
            _meterFactory = meterFactory;
        }

        public async Task Invoke()
        {
            var meter = _meterFactory.Create("Notifier.Blazor");
            try
            {
                var startedInstrument = meter.CreateUpDownCounter<int>($"Notifier.Blazor.Jobs.{typeof(T).Name}.Started");
                startedInstrument.Add(1);
                _logger.LogInformation("Job execution started");
                await Run();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Job execution failed");
                var failedInstrument = meter.CreateUpDownCounter<int>($"Notifier.Blazor.Jobs.{typeof(T).Name}.Failed");
                failedInstrument.Add(1);
            }
            _logger.LogInformation("Job execution finished");
            var succeededInstrument = meter.CreateUpDownCounter<int>($"Notifier.Blazor.Jobs.{typeof(T).Name}.Succeeded");
            succeededInstrument.Add(1);
        }

        protected abstract Task Run();
    }
}
