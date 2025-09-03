using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class VillaEvent : HallProperty<VillaEventDetails>
{
    public VillaEvent() : base(Guid.NewGuid()) { }

    public VillaEvent(
        string title,
        string description,
        Location location,
        Money price,
        VillaEventDetails details,
        Guid businessOwnerId,
        bool isAd,
        ViedeoUrl viedeoUrl,
        List<DateTime>? unavailables = null,
        Discount? discount = null)
        : base(
            title,
            description,
            location,
            price,
            PropertyType.Hall,
            businessOwnerId,
            isAd,
            details,
            viedeoUrl,
            unavailables,
            discount)
    { }
}