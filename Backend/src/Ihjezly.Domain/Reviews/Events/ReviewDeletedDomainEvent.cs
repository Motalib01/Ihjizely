using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reviews.Events;

public sealed record ReviewDeletedDomainEvent(Guid ReviewId, Guid PropertyId, Guid UserId) : IDomainEvent;