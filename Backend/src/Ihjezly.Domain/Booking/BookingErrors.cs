using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Booking;

public static class BookingErrors
{
    //dodo: apply all errors in use cases
    public static readonly Error NotFound = new(
        "Booking.NotFound",
        "The booking with the specified identifier was not found.");

    public static readonly Error Overlapping = new(
        "Booking.Overlapping",
        "The selected booking dates overlap with an existing booking.");

    public static readonly Error NotPending = new(
        "Booking.NotPending",
        "The booking is not in a pending state.");

    public static readonly Error NotConfirmed = new(
        "Booking.NotConfirmed",
        "The booking is not in a confirmed state.");

    public static readonly Error AlreadyStarted = new(
        "Booking.AlreadyStarted",
        "The booking has already started and cannot be modified.");

    public static readonly Error AlreadyCompleted = new(
        "Booking.AlreadyCompleted",
        "The booking has already been completed.");

    public static readonly Error AlreadyCancelled = new(
        "Booking.AlreadyCancelled",
        "The booking has already been cancelled.");

    public static readonly Error Validation = new(
        "Booking.InvalidStatu",
        "Unsupported status change.");
}