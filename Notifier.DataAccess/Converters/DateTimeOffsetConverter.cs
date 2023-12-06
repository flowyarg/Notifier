using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Notifier.DataAccess.Converters
{
    public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public DateTimeOffsetConverter()
            : base(
                d => d.ToUniversalTime(),
                d => d.ToLocalTime())
        {}
    }
}
