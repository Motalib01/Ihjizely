using Ihjezly.Domain.Abstractions;
using MediatR;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId, UserRole Role) : IDomainEvent, INotification;