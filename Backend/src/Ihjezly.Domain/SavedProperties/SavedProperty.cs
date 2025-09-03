using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.SavedProperties.Events;

namespace Ihjezly.Domain.SavedProperties;

public sealed class SavedProperty : Entity
{
    public Guid UserId { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTime SavedAt { get; private set; }

    private SavedProperty() : base(Guid.NewGuid()) { }

    private SavedProperty(Guid userId, Guid propertyId) : this()
    {
        UserId = userId;
        PropertyId = propertyId;
        SavedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SavedPropertySavedDomainEvent(Id, UserId, PropertyId, SavedAt));
    }

    public static SavedProperty Save(Guid userId, Guid propertyId)
        => new SavedProperty(userId, propertyId);

    public void Remove()
    {
        RaiseDomainEvent(new SavedPropertyRemovedDomainEvent(Id, UserId, PropertyId));
    }
}