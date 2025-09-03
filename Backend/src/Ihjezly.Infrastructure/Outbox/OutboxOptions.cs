namespace Ihjezly.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int IntervalInSeconds { get; init; } = 10; // default 10 seconds
    public int BatchSize { get; init; } = 20; // default 20 per batch
}