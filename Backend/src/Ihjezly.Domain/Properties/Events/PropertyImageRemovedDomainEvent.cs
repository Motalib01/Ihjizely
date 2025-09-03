using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Properties.Events;

public sealed record PropertyImageRemovedDomainEvent(Guid id, string url) : IDomainEvent;