using Asp.Versioning;
using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Booking.CreateBooking;
using Ihjezly.Application.Booking.DeleteBooking;
using Ihjezly.Application.Booking.GetAllBookingsQuery;
using Ihjezly.Application.Booking.GetBookingById;
using Ihjezly.Application.Booking.GetBookingsByBusinessOwnerId;
using Ihjezly.Application.Booking.GetBookingsByClientId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISender _mediator;

    public BookingsController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllBookingsQuery());
        return Ok(result.Value);
    }

    // GET: api/bookings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) =>
        Ok(await _mediator.Send(new GetBookingByIdQuery(id)));

    [HttpGet("by-business-owner/{businessOwnerId}")]
    public async Task<IActionResult> GetByBusinessOwnerId(Guid businessOwnerId)
    {
        var result = await _mediator.Send(new GetBookingsByBusinessOwnerIdQuery(businessOwnerId));
        return Ok(result.Value);
    }

    [HttpGet("by-client/{clientId}")]
    public async Task<IActionResult> GetByClientId(Guid clientId)
    {
        var result = await _mediator.Send(new GetBookingsByClientIdQuery(clientId));
        return Ok(result.Value);
    }


    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command) =>
        Ok(await _mediator.Send(command));

    [HttpPatch("{bookingId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid bookingId, [FromBody] UpdateBookingStatusRequest request)
    {
        var command = new UpdateBookingStatusCommand(bookingId, request.NewStatus);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBookingCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

}