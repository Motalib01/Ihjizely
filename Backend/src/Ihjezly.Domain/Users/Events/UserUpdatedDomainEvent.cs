using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserUpdatedDomainEvent(Guid UserId) : IDomainEvent;