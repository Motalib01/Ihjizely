using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Properties.Events;

public sealed record PropertyImagesClearedDomainEvent(Guid id) : IDomainEvent;