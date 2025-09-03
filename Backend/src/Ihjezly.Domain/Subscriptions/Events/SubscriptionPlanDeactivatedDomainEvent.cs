using Ihjezly.Domain.Abstractions;

public sealed record SubscriptionPlanDeactivatedDomainEvent(Guid SubscriptionPlanId) : IDomainEvent;