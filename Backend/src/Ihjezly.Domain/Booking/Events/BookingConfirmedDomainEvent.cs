using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Booking.Events;

public sealed record BookingConfirmedDomainEvent(Guid BookingId) : IDomainEvent;