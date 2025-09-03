using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Application.Properties;

public sealed record CreatePropertyCommand<TProperty, TDetails>(
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
    List<string>? Images,
    DiscountDto? Discount,
    List<DateTime> Unavailables,
    List<Facility>? Facilities
) : ICommand<Guid>;