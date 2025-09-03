using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class EventHallLarge : HallProperty<EventHallLargeDetails>
{
    public override bool SupportsFacilities => false;

    public EventHallLarge() : base(Guid.NewGuid()) { }

    public EventHallLarge(
        string title,
        string description,
        Location location,
        Money price,
        EventHallLargeDetails details,
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
            discount
        )
    { }
}