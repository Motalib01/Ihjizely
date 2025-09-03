namespace Ihjezly.Application.Properties.DTO;

public sealed record LocationDto(
    string City,
    string State,
    string Country,
    double Latitude,
    double Longitude
);