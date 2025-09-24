using Asp.Versioning;
using Ihjezly.Application.Properties.Location.CreateLocation;
using Ihjezly.Application.Properties.Location.DeleteLocation;
using Ihjezly.Application.Properties.Location.GetLocationById;
using Ihjezly.Application.Properties.Location.GetSelectableLocations;
using Ihjezly.Application.Properties.Location.UpdateLocation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ISender _mediator;

    public LocationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteLocationCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationCommand request)
    {
        var command = request with { Id = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetSelectableLocationsQuery());
        return Ok(result.Value);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetLocationByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value }, null)
            : BadRequest(result.Error);
    }
}