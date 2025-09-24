using Asp.Versioning;
using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Transactions.AddFunds;
using Ihjezly.Application.Transactions.AdminAddFunds;
using Ihjezly.Application.Transactions.DeductFunds;
using Ihjezly.Application.Transactions.GetByUser;
using Ihjezly.Application.Transactions.ListTransactionsForWallet;
using Ihjezly.Application.Wallets.GetWalletByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserContext _userContext;

    public TransactionsController(ISender mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    // GET: api/transactions
    [HttpGet]
    public async Task<IActionResult> GetMyTransactions()
    {
        var userId = _userContext.UserId;

        var walletResult = await _mediator.Send(new GetWalletByUserQuery(userId));
        if (!walletResult.IsSuccess)
            return NotFound("walletResult.Errors");

        var result = await _mediator.Send(new ListTransactionsForWalletQuery(walletResult.Value.WalletId));
        return result.IsSuccess ? Ok(result.Value) : NotFound("result.Errors");
    }

    // POST: api/transactions/add-funds
    [HttpPost("add-funds")]
    public async Task<IActionResult> AddFunds([FromBody] AddFundsTransactionRequest request)
    {
        var userId = _userContext.UserId;

        var walletResult = await _mediator.Send(new GetWalletByUserQuery(userId));
        if (!walletResult.IsSuccess)
            return NotFound(walletResult.Error); 

        var command = new AddFundsTransactionCommand(
            walletResult.Value.WalletId,
            request.Amount,
            request.Currency,
            request.Description,
            request.PaymentMethod 
        );

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }

    [HttpPost("admin/add-funds/{walletId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminAddFunds(Guid walletId, [FromBody] AdminAddFundsRequest request)
    {
        var command = new AdminAddFundsCommand(
            walletId,
            request.Amount,
            request.Currency,
            request.Description
        );

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }


    // POST: api/transactions/deduct-funds
    [HttpPost("deduct-funds")]
    public async Task<IActionResult> DeductFunds([FromBody] DeductFundsTransactionRequest request)
    {
        var userId = _userContext.UserId;

        var walletResult = await _mediator.Send(new GetWalletByUserQuery(userId));
        if (!walletResult.IsSuccess)
            return NotFound("walletResult.Errors");

        var command = new DeductFundsTransactionCommand(walletResult.Value.WalletId, request.Amount, request.Currency, request.Description);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest("result.Errors");
    }


    // GET: api/transactions/by-user/{userId}
    [HttpGet("by-user/{userId}")]
    [AllowAnonymous] // Optional, depending on security
    public async Task<IActionResult> GetTransactionsByUserId(Guid userId)
    {
        var result = await _mediator.Send(new GetTransactionsByUserIdQuery(userId));
        return result.IsSuccess ? Ok(result.Value) : NotFound(ApiError.From(result.Error));
    }


}