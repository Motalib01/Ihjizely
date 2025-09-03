using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class MeetingRoom : HallProperty<MeetingRoomDetails>
{
    public MeetingRoom() : base(Guid.NewGuid()) { }

    public MeetingRoom(
        string title,
        string description,
        Location location,
        Money price,
        Guid businessOwnerId,
        bool isAd,
        ViedeoUrl viedeoUrl,
        MeetingRoomDetails details,
        List<DateTime>? unavailables = null,
        Discount? discount = null
        )
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