using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;

namespace Ihjezly.Application.Booking.GetBookingsByBusinessOwnerId;

public sealed record GetBookingsByBusinessOwnerIdQuery(Guid BusinessOwnerId) : IQuery<List<BookingDto>>;