using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserPasswordChangedDomainEvent(Guid UserId) : IDomainEvent;