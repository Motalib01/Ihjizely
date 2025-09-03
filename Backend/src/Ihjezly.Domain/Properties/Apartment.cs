using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class Apartment : ResidenceProperty<ApartmentDetails>
{
    public Apartment() : base(Guid.NewGuid()) { }

    public Apartment(
        string title,
        string description,
        Location location,
        Money price,
        Guid businessOwnerId,
        bool isAd,
        ApartmentDetails details,
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

