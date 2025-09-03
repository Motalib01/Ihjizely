using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Booking.DeleteBooking;

public sealed record DeleteBookingCommand(Guid BookingId) : ICommand;