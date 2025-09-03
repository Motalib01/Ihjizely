using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record SubscriptionPlanUpdatedDomainEvent(Guid SubscriptionPlanId, string Name, TimeSpan Duration, Money Price) : IDomainEvent;