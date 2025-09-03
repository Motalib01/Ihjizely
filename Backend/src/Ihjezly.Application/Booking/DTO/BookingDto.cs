namespace Ihjezly.Application.Booking.DTO;

public sealed record BookingDto(
    Guid Id,
    Guid ClientId,
    string Name,
    string PhoneNumber,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice,
    string Currency,
    string Status,
    DateTime ReservedAt
);
