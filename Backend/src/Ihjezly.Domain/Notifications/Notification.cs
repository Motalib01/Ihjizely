using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications.Events;

namespace Ihjezly.Domain.Notifications;

//todo: notification for booking, review, saved property, user, properties ...

public sealed class Notification : Entity
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }
    public bool IsRead { get; private set; }

    private Notification() : base(Guid.NewGuid()) { }

    private Notification(Guid userId, string message) : this()
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Notification message cannot be empty.");

        UserId = userId;
        Message = message;
        SentAt = DateTime.UtcNow;
        IsRead = false;

        RaiseDomainEvent(new NotificationCreatedDomainEvent(Id, UserId, Message, SentAt));
    }
    public static Notification Create(Guid userId, string message)
        => new Notification(userId, message);

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            RaiseDomainEvent(new NotificationMarkedAsReadDomainEvent(Id, UserId));
        }
    }
    public void Delete()
    {
        RaiseDomainEvent(new NotificationDeletedDomainEvent(Id, UserId));
    }
}