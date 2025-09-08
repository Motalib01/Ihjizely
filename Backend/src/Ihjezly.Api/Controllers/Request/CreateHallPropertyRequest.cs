using System.Text.Json.Serialization;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Api.Controllers.Request;

public class CreateHallPropertyRequest<TProperty, TDetails>
    where TProperty : HallProperty<TDetails>, new()
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = default!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "LYD";
    public PropertyType Type { get; set; }
    public TDetails Details { get; set; } = default!;
    public ViedeoUrl? ViedeoUrl { get; set; }
    public bool IsAd { get; set; } = false;
    public DiscountDto? Discount { get; set; }
    public List<DateTime>? Unavailables { get; set; }

    [JsonIgnore]
    public Guid BusinessOwnerId { get; set; }

    [JsonIgnore]
    public List<Image>? Images { get; set; }

    public CreateHallPropertyCommand<TProperty, TDetails> ToCommand()
    {
        return new CreateHallPropertyCommand<TProperty, TDetails>(
            Title,
            Description,
            Location,
            Price,
            Currency,
            Details,
            Type,
            BusinessOwnerId,
            IsAd,
            ViedeoUrl,
            Images,
            Discount,
            Unavailables ?? new List<DateTime>()
        );
    }
}