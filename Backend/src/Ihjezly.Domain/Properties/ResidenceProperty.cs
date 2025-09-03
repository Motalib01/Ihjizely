using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public abstract class ResidenceProperty<TDetails> : PropertyWithDetails<TDetails>
{
    public override bool SupportsFacilities => true;

    protected ResidenceProperty(Guid id) : base(id) { }

    protected ResidenceProperty(
        string title,
        string description,
        Location location,
        Money price,
        PropertyType type,
        Guid businessOwnerId,
        bool isAd,
        TDetails details,
        ViedeoUrl viedeoUrl,
        List<DateTime>? unavailbles = null,
        Discount? discount = null,
        List<Facility>? facilities = null)
        : base(title, description, location, price, type, businessOwnerId, isAd, details, viedeoUrl, unavailbles, discount, facilities)
    {
    }

}