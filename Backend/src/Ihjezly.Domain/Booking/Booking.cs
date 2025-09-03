using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking.Events;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Booking;

public sealed class Booking : Entity
{
    public Guid ClientId { get; private set; }
    public Guid PropertyId { get; private set; }
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime ReservedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Booking() : base(Guid.NewGuid()) { }

    private Booking(Guid clientId, string name, string phoneNumber, Property property, DateTime startDate, DateTime endDate)
        : this()
    {
        if (startDate >= endDate) throw new ArgumentException("End date must be after start date.");

        ClientId = clientId;
        Name = name;
        PhoneNumber = phoneNumber; 
        PropertyId = property.Id;
        StartDate = startDate;
        EndDate = endDate;
        TotalPrice = PricingService.CalculateTotalPrice(property, startDate, endDate);
        Status = BookingStatus.Pending;
        ReservedAt = DateTime.UtcNow;

        RaiseDomainEvent(new BookingCreatedDomainEvent(Id, ClientId, PropertyId, StartDate, EndDate, TotalPrice));
    }

    public static Booking Reserve(Guid clientId, string name, string phoneNumber, Guid propertyId, Property property, DateTime startDate, DateTime endDate)
    {
        var totalPrice = PricingService.CalculateTotalPrice(property, startDate, endDate);

        var booking = new Booking
        {
            ClientId = clientId,
            PropertyId = propertyId,
            Name = name,
            PhoneNumber = phoneNumber,
            StartDate = startDate,
            EndDate = endDate,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            ReservedAt = DateTime.UtcNow
        };

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(
            booking.Id,
            booking.ClientId,
            booking.PropertyId,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice
        ));

        return booking;
    }

    public void ChangeStatusTo(BookingStatus newStatus)
    {
        if (Status == newStatus)
            throw new InvalidOperationException("Booking already has the desired status.");

        switch (newStatus)
        {
            case BookingStatus.Confirmed:
                Confirm();
                break;
            case BookingStatus.Cancelled:
                Cancel();
                break;
            case BookingStatus.Rejected:
                Reject();
                break;
            case BookingStatus.Completed:
                Complete();
                break;
            case BookingStatus.Pending:
                throw new InvalidOperationException("Cannot revert to Pending.");
            default:
                throw new InvalidOperationException("Unsupported status change.");
        }
    }
    public void UpdateStatus(BookingStatus newStatus)
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status after completion or refusal.");

        Status = newStatus;
    }

    private void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));
    }

    private void Cancel()
    {
        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("Completed bookings cannot be cancelled.");

        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BookingCancelledDomainEvent(Id));
    }

    private void Reject()
    {
        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("Completed bookings cannot be rejected.");

        Status = BookingStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BookingRejectedDomainEvent(Id));
    }

    private void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed.");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BookingCompletedDomainEvent(Id));
    }
}
