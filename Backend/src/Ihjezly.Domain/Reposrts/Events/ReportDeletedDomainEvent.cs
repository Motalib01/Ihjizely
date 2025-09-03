using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reposrts.Events;

public sealed record ReportDeletedDomainEvent(Guid ReportId, Guid UserId) : IDomainEvent;