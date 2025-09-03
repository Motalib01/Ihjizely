using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Booking.Events;

public sealed record BookingReservedDomainEvent(
    Guid BookingId,
    Guid ClientId,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    Money TotalPrice
) : IDomainEvent;