using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Helpers.Metrics
{
    public static class MetricsExtensions
    {
        public static IDisposable Measure(this Meter meter, string name)
        {
            var counter = meter.CreateCounter<long>(name);

            return new DurationMeasurement(counter);
        }
        
        public static IDisposable MeasureHS(this Meter meter, string name)
        {
            var histogram = meter.CreateHistogram<long>(name);

            return new HistogramMeasurement(histogram);
        }
    }

    public sealed class DurationMeasurement : IDisposable
    {
        private readonly DateTimeOffset _startTime;
        private readonly Counter<long> _counter;

        public DurationMeasurement(Counter<long> counter)
        {
            _counter = counter;
            _startTime = DateTimeOffset.UtcNow;
        }

        public void Dispose() => _counter.Add((long)(DateTimeOffset.UtcNow - _startTime).TotalNanoseconds);
    }

    public sealed class HistogramMeasurement : IDisposable
    {
        private readonly DateTimeOffset _startTime;
        private readonly Histogram<long> _histogram;

        public HistogramMeasurement(Histogram<long> histogram)
        {
            _startTime = DateTimeOffset.UtcNow;
            _histogram = histogram;
        }

        public void Dispose() => _histogram.Record((long)(DateTimeOffset.UtcNow - _startTime).TotalNanoseconds);
    }
}
