using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Notifications.Events;

public sealed record NotificationCreatedDomainEvent(
    Guid NotificationId,
    Guid UserId,
    string Message,
    DateTime SentAt
) : IDomainEvent;