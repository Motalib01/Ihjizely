using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

public sealed record CreateResidencePropertyCommand<TResidence, TDetails>(
    string Title,
    string Description,
    LocationDto Location,
    decimal Price,
    string Currency,
    TDetails Details,
    ViedeoUrl ViedeoUrl,
    PropertyType Type,
    Guid BusinessOwnerId,
    bool IsAd,
    List<Image>? Images,
    DiscountDto? Discount,
    List<DateTime> Unavailables,
    List<Facility>? Facilities
) : ICommand<Guid>
    where TResidence : ResidenceProperty<TDetails>, new();