using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.SavedProperties.Events;

public sealed record SavedPropertyRemovedDomainEvent(Guid SavedPropertyId, Guid UserId, Guid PropertyId) : IDomainEvent;