using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record SubscriptionCreatedDomainEvent(
    Guid SubscriptionId,
    Guid BusinessOwnerId,
    Guid PlanId,
    DateTime StartDate,
    DateTime EndDate,
    Money Price
) : IDomainEvent;