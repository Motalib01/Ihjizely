using Ihjezly.Domain.Abstractions;
using MediatR;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserUnblockedDomainEvent : IDomainEvent, INotification
{
    public Guid UserId { get; }

    public UserUnblockedDomainEvent(Guid userId)
    {
        UserId = userId;
    }
}