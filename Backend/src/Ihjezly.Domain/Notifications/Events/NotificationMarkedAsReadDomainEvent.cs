using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Notifications.Events;

public sealed record NotificationMarkedAsReadDomainEvent(Guid NotificationId, Guid UserId) : IDomainEvent;