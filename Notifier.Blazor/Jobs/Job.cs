using Coravel.Invocable;

namespace Notifier.Blazor.Jobs
{
    internal abstract class Job<T> : IInvocable
    {
        protected readonly ILogger<T> _logger;

        public Job(ILogger<T> logger)
        {
            _logger = logger;
        }

        public async Task Invoke()
        {
            try
            {
                _logger.LogInformation("Job execution started");
                await Run();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Job execution failed");
            }
            _logger.LogInformation("Job execution finished");
        }

        protected abstract Task Run();
    }
}           
