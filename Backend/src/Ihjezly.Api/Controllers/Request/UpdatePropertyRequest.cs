using System.Text.Json.Serialization;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Application.Properties.UpdateProperty;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Api.Controllers.Request;

public class UpdatePropertyRequest<TProperty, TDetails>
    where TProperty : PropertyWithDetails<TDetails>
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = default!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "LYD";
    public PropertyType Type { get; set; }
    public TDetails Details { get; set; } = default!;
    public DiscountDto? Discount { get; set; }
    public List<Facility>? Facilities { get; set; }
    public ViedeoUrl? ViedeoUrl { get; set; } 
    public List<DateTime>? Unavailables { get; set; }

    [JsonIgnore]
    public Guid BusinessOwnerId { get; set; }

    public List<string>? NewImages { get; set; }
    public List<string>? DeletedImages { get; set; }

    public UpdatePropertyCommand<TProperty, TDetails> ToCommand()
    {
        return new(
            PropertyId,
            Title,
            Description,
            Location,
            Price,
            Currency,
            Type,
            Details,
            Discount,
            Unavailables,
            ViedeoUrl,
            Facilities,
            NewImages,
            DeletedImages
        );
    }

}