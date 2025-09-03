using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.PropertySearch;

public sealed record PropertySearchRequest
{
    public string? Title { get; init; }
    public List<string>? Types { get; set; }
    public decimal? MinPrice { get; init; }     
    public decimal? MaxPrice { get; init; }     
    public string? Currency { get; init; }      
    public PropertyStatus? Status { get; init; } 
    public Facility ? Facility { get; init; }

    public Guid? BusinessOwnerId { get; init; }  
    public string? City { get; init; }
    public string? State { get; init; }
}