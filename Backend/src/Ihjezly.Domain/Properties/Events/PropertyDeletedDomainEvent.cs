using Ihjezly.Domain.Abstractions;

public sealed record PropertyDeletedDomainEvent(Guid PropertyId) : IDomainEvent;