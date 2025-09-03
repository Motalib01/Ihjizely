using System.Text.Json;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.DTO;

public sealed record ResidencePropertyDto(
    Guid Id,
    string Title,
    string Description,
    LocationDto Location,
    decimal Price,
    string Currency,
    DiscountDto? Discount,
    bool IsAd,
    List<FacilityDto> Facilities,
    PropertyType Type,
    PropertyStatus Status,
    ViedeoUrl ViedeoUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<DateTime> Unavailables,
    JsonElement? Details,
    List<ImageDto> Images
);