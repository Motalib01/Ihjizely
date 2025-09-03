using Ihjezly.Application.Payments.Edfali;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EdfaliController : ControllerBase
{
    private readonly IMediator _mediator;

    public EdfaliController(IMediator mediator)
    {
        _mediator = mediator;
    }

   
    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiateEdfaliTransferCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { SessionIdOrError = result });
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmEdfaliTransferCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { Result = result });
    }
}