using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class RestHouse : ResidenceProperty<RestHouseDetails>
{
    public RestHouse() : base(Guid.NewGuid()) { }

    public RestHouse(
        string title,
        string description,
        Location location,
        Money price,
        RestHouseDetails details,
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