using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Booking;

public sealed record UpdateBookingStatusCommand(Guid BookingId, BookingStatus NewStatus) : ICommand;