using Asp.Versioning;
using Ihjezly.Application.Reports.CreateReport;
using Ihjezly.Application.Reports.DeleteReport;
using Ihjezly.Application.Reports.GetAllReports;
using Ihjezly.Application.Reports.GetReportById;
using Ihjezly.Application.Reports.ReplayReport;
using Ihjezly.Application.Reports.UpdateReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _mediator;

    public ReportsController(ISender mediator) => _mediator = mediator;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReportCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var enrichedCommand = command with { UserId = parsedUserId };

        var result = await _mediator.Send(enrichedCommand);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllReportsQuery());
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _mediator.Send(new GetReportByIdQuery(id)));

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReportCommand command)
    {
        if (id != command.ReportId)
            return BadRequest("Route ID and body ID must match.");

        return Ok(await _mediator.Send(command));
    }

    [HttpPost("{id}/replay")]
    public async Task<IActionResult> Replay(Guid id, [FromBody] string replay)
    {
        if (string.IsNullOrWhiteSpace(replay))
            return BadRequest("Replay cannot be empty.");

        var command = new ReplayReportCommand(id, replay);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        Ok(await _mediator.Send(new DeleteReportCommand(id)));
}