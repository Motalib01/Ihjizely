using Ihjezly.Domain.Booking;

namespace Ihjezly.Api.Controllers.Request;

public class UpdateBookingStatusRequest
{
    public BookingStatus NewStatus { get; set; }
}