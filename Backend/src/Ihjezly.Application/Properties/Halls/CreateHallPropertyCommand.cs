using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

public sealed record CreateHallPropertyCommand<THall, TDetails>(
    string Title,
    string Description,
    LocationDto Location,
    decimal Price,
    string Currency,
    TDetails Details,
    PropertyType Type,
    Guid BusinessOwnerId,
    bool IsAd,
    ViedeoUrl ViedeoUrl,
    List<Image>? Images,
    DiscountDto? Discount,
    List<DateTime> Unavailables
) : ICommand<Guid>
    where THall : HallProperty<TDetails>, new();