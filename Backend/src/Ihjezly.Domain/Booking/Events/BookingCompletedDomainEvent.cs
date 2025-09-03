using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Booking.Events;

public sealed record BookingCompletedDomainEvent(Guid BookingId) : IDomainEvent;