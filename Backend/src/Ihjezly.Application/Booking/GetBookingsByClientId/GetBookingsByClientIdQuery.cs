using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;

namespace Ihjezly.Application.Booking.GetBookingsByClientId;

public sealed record GetBookingsByClientIdQuery(Guid ClientId) : IQuery<List<BookingDto>>;