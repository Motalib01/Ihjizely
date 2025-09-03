using Ihjezly.Domain.Abstractions;

public sealed record PropertyUpdatedDomainEvent(Guid PropertyId) : IDomainEvent;