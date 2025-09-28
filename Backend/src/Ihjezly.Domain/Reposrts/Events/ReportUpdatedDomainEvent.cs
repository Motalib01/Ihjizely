using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reposrts.Events;

public sealed record ReportUpdatedDomainEvent(Guid ReportId,
    Guid UserId,
    string Reason,
    string Content,
    bool IsRead) : IDomainEvent;