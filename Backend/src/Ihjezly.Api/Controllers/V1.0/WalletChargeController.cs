using Asp.Versioning;
using Ihjezly.Application.Payments.Masarat;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WalletChargeController : ControllerBase
{
    private readonly ISender _sender;

    public WalletChargeController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("charge/initiate")]
    public async Task<IActionResult> InitiateCharge([FromBody] StartMasaratWalletChargeCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess
            ? Ok(new { transactionId = result.Value })
            : BadRequest(result.Error);
    }

    [HttpPost("charge/confirm")]
    public async Task<IActionResult> ConfirmCharge([FromBody] ConfirmMasaratWalletChargeCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess
            ? Ok("Wallet charged successfully")
            : BadRequest(result.Error);
    }
}
