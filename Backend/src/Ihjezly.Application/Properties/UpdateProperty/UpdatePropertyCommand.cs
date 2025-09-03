using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.UpdateProperty;
public sealed record UpdatePropertyCommand<TProperty, TDetails>(
    Guid PropertyId,
    string Title,
    string Description,
    LocationDto Location,
    decimal Price,
    string Currency,
    PropertyType Type,
    TDetails Details,
    DiscountDto? Discount,
    List<DateTime> Unavailables, // ✅ fix spelling
    ViedeoUrl VideoUrl,          // ✅ fix name to match usage
    List<Facility>? Facilities,
    List<string>? NewImages,
    List<string>? DeletedImages
) : ICommand
    where TProperty : PropertyWithDetails<TDetails>;
