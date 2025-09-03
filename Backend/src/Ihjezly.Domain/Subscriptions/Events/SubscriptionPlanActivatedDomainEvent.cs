using Ihjezly.Domain.Abstractions;

public sealed record SubscriptionPlanActivatedDomainEvent(Guid SubscriptionPlanId) : IDomainEvent;