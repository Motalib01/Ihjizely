using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Properties;

public sealed class SelectableLocation : Entity
{
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string Country { get; private set; } = default!;

    private SelectableLocation() : base(Guid.NewGuid())
    {
    }

    public SelectableLocation(string city, string state, string country)
        : this()
    {
        City = city;
        State = state;
        Country = country;
    }

    public void Update(string city, string state, string country)
    {
        City = city;
        State = state;
        Country = country;
    }
}