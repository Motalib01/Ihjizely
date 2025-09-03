using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

public sealed class EventHallSmall : HallProperty<EventHallSmallDetails>
{
    public override bool SupportsFacilities => false;

    public EventHallSmall() : base(Guid.NewGuid()) { }

    public EventHallSmall(
        string title,
        string description,
        Location location,
        Money price,
        EventHallSmallDetails details,
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