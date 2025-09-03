using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public string Type { get; init; } = default!;
    public string Content { get; init; } = default!;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }

    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    public void MarkAsProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
    }

}