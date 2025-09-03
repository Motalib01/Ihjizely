using Ihjezly.Domain.Abstractions;
using MediatR;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserBlockedDomainEvent : IDomainEvent, INotification
{
    public Guid UserId { get; }
    public string Reason { get; }

    public UserBlockedDomainEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
    }
}