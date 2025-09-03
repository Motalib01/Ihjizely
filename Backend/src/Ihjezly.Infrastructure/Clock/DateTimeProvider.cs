using Ihjezly.Application.Abstractions.Clock;

namespace Ihjezly.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}