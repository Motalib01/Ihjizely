using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reposrts.Events;

public sealed record ReportCreatedDomainEvent(Guid ReportId,
    Guid UserId,
    string Content,
    string Reason, bool IsRead,
    DateTime CreatedAt) : IDomainEvent;