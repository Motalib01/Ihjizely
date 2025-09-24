using Asp.Versioning;
using Ihjezly.Application.SavedProperties.GetSavedPropertiesForUser;
using Ihjezly.Application.SavedProperties.RemoveSavedProperty;
using Ihjezly.Application.SavedProperties.SaveProperty;
using Ihjezly.Domain.SavedProperties;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SavedPropertiesController : ControllerBase
{
    private readonly ISender _mediator;

    public SavedPropertiesController(ISender mediator) => _mediator = mediator;

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySavedProperties()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var result = await _mediator.Send(new GetSavedPropertiesForUserQuery(parsedUserId));
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SavePropertyCommand command) =>
        Ok(await _mediator.Send(command));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var result = await _mediator.Send(new RemoveSavedPropertyCommand(id, parsedUserId));

        if (result.IsSuccess) return NoContent();

        if (result.Error == SavedPropertyErrors.Unauthorized)
            return Forbid();

        return NotFound();
    }


}