using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record SubscriptionRenewedDomainEvent(
    Guid SubscriptionId,
    Guid BusinessOwnerId,
    Guid PlanId,
    DateTime EndDate,
    Money Price
) : IDomainEvent;