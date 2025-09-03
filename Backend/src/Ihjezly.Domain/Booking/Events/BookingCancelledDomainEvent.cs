using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Booking.Events;

public sealed record BookingCancelledDomainEvent(Guid BookingId) : IDomainEvent;