using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;
using System.Text.Json;

namespace Ihjezly.Application.Properties.DTO;

public static class PropertyMappings
{
    // ===== Location =====
    public static Domain.Shared.Location ToDomain(this LocationDto dto) =>
        new(dto.City, dto.State, dto.Country, dto.Latitude, dto.Longitude);

    public static LocationDto ToDto(this Domain.Shared.Location domain) =>
        new(domain.City, domain.State, domain.Country, domain.Latitude, domain.Longitude);

    // ===== Discount =====
    public static Discount ToDomain(this DiscountDto dto) =>
        new(dto.Value);

    public static DiscountDto ToDto(this Discount domain) =>
        new(domain.Value);

    // ===== Facility =====
    public static Facility ToDomain(this FacilityDto dto) =>
        Enum.Parse<Facility>(dto.Name);

    public static FacilityDto ToDto(this Facility domain) =>
        new(domain.ToString());

    // ===== Image =====
    public static ImageDto ToDto(this Image image) =>
        new(image.Url);

    // ===== Generic PropertyWithDetails<T> Mapping =====
    public static PropertyDto ToDto<TDetails>(this PropertyWithDetails<TDetails> property, string businessOwnerFirstName, string businessOwnerLastName)
    {
        var facilities = property.SupportsFacilities
            ? property.Facilities.Select(f => f.ToDto()).ToList()
            : new List<FacilityDto>();

        return new PropertyDto(
            property.Id,
            property.Title,
            property.Description,
            property.Location.ToDto(),
            property.Price.Amount,
            property.Price.Currency.Code,
            property.Discount?.ToDto(),
            property.IsAd,
            facilities,
            property.Type,
            property.Status,
            property.ViedeoUrl,
            property.CreatedAt,
            property.UpdatedAt,
            property.Unavailbles ?? new List<DateTime>(),
            JsonSerializer.SerializeToElement(property.Details),
            property.Images.Select(i => i.ToDto()).ToList(),
            property.BusinessOwnerId,
            businessOwnerFirstName,
            businessOwnerLastName

        );
    }

    // ===== Base Property fallback =====
    public static PropertyDto ToDto(this Property property, string businessOwnerFirsName, string businessOwnerLastName) =>
        property switch
        {
            Apartment a => a.ToDto(businessOwnerFirsName, businessOwnerLastName),
            Chalet c => c.ToDto(businessOwnerFirsName, businessOwnerLastName),
            HotelRoom h => h.ToDto(businessOwnerFirsName, businessOwnerLastName),
            HotelApartment ha => ha.ToDto(businessOwnerFirsName, businessOwnerLastName),
            EventHallLarge l => l.ToDto(businessOwnerFirsName, businessOwnerLastName),
            EventHallSmall s => s.ToDto(businessOwnerFirsName, businessOwnerLastName),
            MeetingRoom m => m.ToDto(businessOwnerFirsName, businessOwnerLastName),
            Resort r => r.ToDto(businessOwnerFirsName, businessOwnerLastName),
            RestHouse rs => rs.ToDto(businessOwnerFirsName, businessOwnerLastName),
            VillaEvent v => v.ToDto(businessOwnerFirsName, businessOwnerLastName),
            _ => throw new InvalidOperationException($"Unknown property type: {property.GetType().Name}")
        };

    // ===== ResidenceProperty Mapping =====
    public static ResidencePropertyDto ToResidenceDto<TDetails>(this ResidenceProperty<TDetails> property) =>
        new(
            property.Id,
            property.Title,
            property.Description,
            property.Location.ToDto(),
            property.Price.Amount,
            property.Price.Currency.Code,
            property.Discount?.ToDto(),
            property.IsAd,
            property.Facilities.Select(f => f.ToDto()).ToList(),
            property.Type,
            property.Status,
            property.ViedeoUrl,
            property.CreatedAt,
            property.UpdatedAt,
            property.Unavailbles ?? new List<DateTime>(),
            JsonSerializer.SerializeToElement(property.Details),
            property.Images.Select(i => i.ToDto()).ToList()
        );

    // ===== HallProperty Mapping =====
    public static HallPropertyDto ToHallDto<TDetails>(this HallProperty<TDetails> property) =>
        new(
            property.Id,
            property.Title,
            property.Description,
            property.Location.ToDto(),
            property.Price.Amount,
            property.Price.Currency.Code,
            property.Discount?.ToDto(),
            property.IsAd,
            property.Type,
            property.Status,
            property.ViedeoUrl,
            property.CreatedAt,
            property.UpdatedAt,
            property.Unavailbles ?? new List<DateTime>(),
            JsonSerializer.SerializeToElement(property.Details),
            property.Images.Select(i => i.ToDto()).ToList()
        );
}
