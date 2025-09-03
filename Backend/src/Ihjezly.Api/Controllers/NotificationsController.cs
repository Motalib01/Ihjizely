using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Notifications.CreateNotification;
using Ihjezly.Application.Notifications.DeleteNotification;
using Ihjezly.Application.Notifications.ListUserNotifications;
using Ihjezly.Application.Notifications.MarkNotificationAsRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _mediator;

    public NotificationsController(ISender mediator) => _mediator = mediator;

    // GET: api/notifications/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAll(Guid userId) =>
        Ok(await _mediator.Send(new ListUserNotificationsQuery(userId)));

    // PATCH: api/notifications/{id}/read
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkNotificationAsReadCommand(id));
        return result.IsSuccess ? NoContent() : NotFound();
    }

    // POST: api/notifications
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
    {
        var command = new CreateNotificationCommand(request.UserId, request.Message);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { userId = request.UserId }, result.Value)
            : BadRequest();
    }

    // DELETE: api/notifications/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteNotificationCommand(id));
        return result.IsSuccess ? NoContent() : NotFound();
    }

    
}