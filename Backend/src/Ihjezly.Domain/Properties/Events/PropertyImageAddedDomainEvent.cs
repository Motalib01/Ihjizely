using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Properties.Events;

public sealed record PropertyImageAddedDomainEvent(Guid id, string url) : IDomainEvent;