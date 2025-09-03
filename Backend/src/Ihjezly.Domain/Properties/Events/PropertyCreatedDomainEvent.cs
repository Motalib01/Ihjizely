using Ihjezly.Domain.Abstractions;

public sealed record PropertyCreatedDomainEvent(Guid PropertyId) : IDomainEvent;