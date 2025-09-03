using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Booking.Events;

public record BookingRejectedDomainEvent(Guid BookingId) : IDomainEvent;