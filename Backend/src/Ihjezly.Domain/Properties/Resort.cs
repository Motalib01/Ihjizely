using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class Resort : ResidenceProperty<ResortDetails>
{
    public Resort() : base(Guid.NewGuid()) { }

    public Resort(
        string title,
        string description,
        Location location,
        Money price,
        ResortDetails details,
        Guid businessOwnerId,
        bool isAd,
        ViedeoUrl viedeoUrl,
        List<DateTime>? unavailables = null,
        Discount? discount = null,
        List<Facility>? facilities = null)
        : base(
            title,
            description,
            location,
            price,
            PropertyType.Residence,
            businessOwnerId,
            isAd,
            details,
            viedeoUrl,
            unavailables,
            discount,
            facilities)
    { }
}