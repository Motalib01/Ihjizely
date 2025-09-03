using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;

namespace Ihjezly.Application.Booking.GetAllBookingsQuery;

public sealed record GetAllBookingsQuery() : IQuery<List<BookingDto>>;