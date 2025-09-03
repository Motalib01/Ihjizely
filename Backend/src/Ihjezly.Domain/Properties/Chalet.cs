using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class Chalet : HallProperty<ChaletDetails>
{
    public Chalet() : base(Guid.NewGuid()) { }

    public Chalet(
        string title,
        string description,
        Location location,
        Money price,
        Guid businessOwnerId,
        bool isAd,
        ChaletDetails details,
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
