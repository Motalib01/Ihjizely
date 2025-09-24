using Asp.Versioning;
using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Subscriptions.ActivateSubscriptionPlan;
using Ihjezly.Application.Subscriptions.CreateSubscription;
using Ihjezly.Application.Subscriptions.CreateSubscriptionPlan;
using Ihjezly.Application.Subscriptions.DeleteSubscriptionPlan;
using Ihjezly.Application.Subscriptions.GetActiveSubscriptionforUser;
using Ihjezly.Application.Subscriptions.GetAllActiveSubscriptions;
using Ihjezly.Application.Subscriptions.GetAllSubscriptionPlans;
using Ihjezly.Application.Subscriptions.GetAllSubscriptions;
using Ihjezly.Application.Subscriptions.GetSubscriptionPlanById;
using Ihjezly.Application.Subscriptions.RenewSubscription;
using Ihjezly.Application.Subscriptions.UpdateSubscriptionPlan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISender _mediator;

    public SubscriptionsController(ISender mediator) => _mediator = mediator;

    // POST: api/subscriptions/plans
    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPlanById), new { id = result.Value }, result.Value);
    }

    // GET: api/subscriptions/plans/{id}
    [HttpGet("plans/{id}")]
    public async Task<IActionResult> GetPlanById(Guid id) =>
        Ok(await _mediator.Send(new GetSubscriptionPlanByIdQuery(id)));

    [Authorize]
    [HttpPatch("plans/{id}/activate")]
    public async Task<IActionResult> ActivatePlan(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var command = new ActivateSubscriptionPlanCommand(id, parsedUserId);
        var result = await _mediator.Send(command);

        return result.IsSuccess ? NoContent() : BadRequest(result.IsFailure);
    }

    // PATCH: api/subscriptions/plans/{planId}
    [HttpPatch("plans/{planId}")]
    public async Task<IActionResult> UpdatePlan(Guid planId, [FromBody] UpdateSubscriptionPlanRequest request)
    {
        if (planId != request.PlanId)
            return BadRequest("Route ID and body ID must match.");

        var command = new UpdateSubscriptionPlanCommand(
            request.PlanId,
            request.Name,
            request.DurationInDays,
            request.Amount,
            request.Currency,
            request.MaxAds   
        );

        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest("result.Errors");
    }


    // POST: api/subscriptions
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("api/subscriptions", result.Value);
    }

    // PATCH: api/subscriptions/{subscriptionId}/renew/{planId}
    [HttpPatch("{subscriptionId}/renew/{planId}")]
    public async Task<IActionResult> Renew(Guid subscriptionId, Guid planId)
    {
        var result = await _mediator.Send(new RenewSubscriptionCommand(subscriptionId, planId));
        return result.IsSuccess ? NoContent() : BadRequest("result.Errors");
    }

    [HttpGet("active/{userId}")]
    public async Task<IActionResult> GetActiveSubscription(Guid userId)
    {
        var result = await _mediator.Send(new GetActiveSubscriptionForUserQuery(userId));

        return Ok(new
        {
            isSuccess = result.IsSuccess,
            isFailure = result.IsFailure,
            error = result.IsFailure ? result.Error : null,
            value = result.IsSuccess ? result.Value : null
        });
    }



    [HttpGet("plans")]
    public async Task<IActionResult> GetAllPlans()
    {
        var result = await _mediator.Send(new GetAllSubscriptionPlansQuery());
        return Ok(result.Value);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        var result = await _mediator.Send(new GetAllSubscriptionsQuery());
        return Ok(result); 
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetAllActiveSubscriptions()
    {
        var result = await _mediator.Send(new GetAllActiveSubscriptionsQuery());

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("plans/{planId}")]
    public async Task<IActionResult> DeletePlan(Guid planId)
    {
        var command = new DeleteSubscriptionPlanCommand(planId);
        var result = await _mediator.Send(command);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}