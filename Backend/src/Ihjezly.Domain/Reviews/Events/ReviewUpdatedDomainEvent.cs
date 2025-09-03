using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reviews.Events;

public sealed record ReviewUpdatedDomainEvent(Guid ReviewId, Guid PropertyId, Guid UserId, int Rating, string Comment) : IDomainEvent;