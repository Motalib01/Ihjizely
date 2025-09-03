using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserDeletedDomainEvent(Guid UserId) : IDomainEvent;