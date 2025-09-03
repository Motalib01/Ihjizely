using System.Text.Json.Serialization;
using Ihjezly.Application.Properties;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Api.Controllers.Request;

public class CreatePropertyRequest<TProperty, TDetails>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = default!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "LYD";
    public PropertyType Type { get; set; }
    public TDetails Details { get; set; } = default!;
    public bool IsAd { get; set; } = false;

    // Fix typo here if possible; otherwise keep consistent with domain model
    public ViedeoUrl? ViedeoUrl { get; set; }

    public DiscountDto? Discount { get; set; }
    public List<Facility>? Facilities { get; set; }
    public List<DateTime>? Unavailables { get; set; }

    [JsonIgnore]
    public Guid BusinessOwnerId { get; set; }

    [JsonIgnore]
    public List<string>? Images { get; set; }

    public CreatePropertyCommand<TProperty, TDetails> ToCommand()
    {
        return new CreatePropertyCommand<TProperty, TDetails>(
            Title,
            Description,
            Location,
            Price,
            Currency,
            Details,
            ViedeoUrl,
            Type,
            BusinessOwnerId,
            IsAd,
            Images,
            Discount,
            Unavailables ?? new List<DateTime>(),
            Facilities ?? new List<Facility>()
        );
    }
}