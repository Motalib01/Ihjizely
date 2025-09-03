using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Domain.Shared;
[Owned]
public sealed record Location
    (string City,
        string State,
        string Country,
        double Latitude,
        double Longitude
        );