using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public sealed class HotelApartment : ResidenceProperty<HotelApartmentDetails>
{
    public HotelApartment() : base(Guid.NewGuid()) { }

    public HotelApartment(
        string title,
        string description,
        Location location,
        Money price,
        HotelApartmentDetails details,
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