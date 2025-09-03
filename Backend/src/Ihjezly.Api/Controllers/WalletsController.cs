using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Wallets.CreateWallet;
using Ihjezly.Application.Wallets.GetAllWallets;
using Ihjezly.Application.Wallets.GetWalletByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserContext _userContext;

    public WalletsController(ISender mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    // POST: api/wallets
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var userId = _userContext.UserId;
        var command = new CreateWalletCommand(userId);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByUserId), new { userId = userId }, result.Value)
            : BadRequest(result.Error);
    }

    // POST: api/wallets/{userId}
    [HttpPost("{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateForUser(Guid userId)
    {
        var result = await _mediator.Send(new CreateWalletCommand(userId));

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByUserId), new { userId = userId }, result.Value)
            : BadRequest(result.Error);
    }

    // GET: api/wallets/user
    [HttpGet("user")]
    public async Task<IActionResult> GetByUserId()
    {
        var userId = _userContext.UserId;
        var result = await _mediator.Send(new GetWalletByUserQuery(userId));

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }

    // GET: api/wallets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllWalletsQuery());

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}