using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;

namespace Ihjezly.Application.Booking.CreateBooking;

public sealed record CreateBookingCommand(
    Guid ClientId,
    string Name,
    string PhoneNumber,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal DailyPrice,
    string Currency
) : ICommand<BookingDto>;