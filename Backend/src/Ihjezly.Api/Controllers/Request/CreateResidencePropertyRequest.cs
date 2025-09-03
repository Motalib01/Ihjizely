using System.Text.Json.Serialization;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Api.Controllers.Request;

public class CreateResidencePropertyRequest<TProperty, TDetails>
    where TProperty : ResidenceProperty<TDetails>, new()
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = default!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "LYD";
    public PropertyType Type { get; set; }
    public TDetails Details { get; set; } = default!;
    public bool IsAd { get; set; } = false;
    public DiscountDto? Discount { get; set; }
    public List<Facility>? Facilities { get; set; }
    public ViedeoUrl? ViedeoUrl { get; set; }
    public List<DateTime>? Unavailables { get; set; }

    [JsonIgnore]
    public Guid BusinessOwnerId { get; set; }

    [JsonIgnore]
    public List<string>? Images { get; set; }

    public CreateResidencePropertyCommand<TProperty, TDetails> ToCommand()
    {
        return new CreateResidencePropertyCommand<TProperty, TDetails>(
            Title: Title,
            Description: Description,
            Location: Location,
            Price: Price,
            Currency: Currency,
            Details: Details,
            ViedeoUrl: ViedeoUrl,
            Type: Type,
            BusinessOwnerId: BusinessOwnerId,
            IsAd: IsAd,
            Images: Images,
            Discount: Discount,
            Unavailables: Unavailables ?? new List<DateTime>(),
            Facilities: Facilities ?? new List<Facility>()
        );
    }
}