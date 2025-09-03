using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public abstract class HallProperty<TDetails> : PropertyWithDetails<TDetails>
{
    public override bool SupportsFacilities => false;

    protected HallProperty(Guid id) : base(id) { }

    protected HallProperty(
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
        Discount? discount = null)
        : base(title, description, location, price, type, businessOwnerId, isAd, details, viedeoUrl, unavailbles, discount, facilities: null)
    {
    }
}