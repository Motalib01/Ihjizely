using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Notifications.Events;

public sealed record NotificationDeletedDomainEvent(Guid NotificationId, Guid UserId) : IDomainEvent;