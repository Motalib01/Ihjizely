using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(Guid ReviewId, Guid PropertyId, Guid UserId, int Rating, string Comment, DateTime CreatedAt) : IDomainEvent;