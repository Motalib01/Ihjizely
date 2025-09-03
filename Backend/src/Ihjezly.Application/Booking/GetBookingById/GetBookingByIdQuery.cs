using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;

namespace Ihjezly.Application.Booking.GetBookingById;

public sealed record GetBookingByIdQuery(Guid BookingId) : IQuery<BookingDto>;