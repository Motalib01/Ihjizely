using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.SavedProperties.Events;

public sealed record SavedPropertySavedDomainEvent(Guid SavedPropertyId, Guid UserId, Guid PropertyId, DateTime SavedAt) : IDomainEvent;