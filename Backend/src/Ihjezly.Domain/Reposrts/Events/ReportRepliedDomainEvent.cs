using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reposrts.Events;

public sealed record ReportRepliedDomainEvent(
    Guid ReportId,
    Guid UserId,
    string Replay,
    DateTime RepliedAt
) : IDomainEvent;