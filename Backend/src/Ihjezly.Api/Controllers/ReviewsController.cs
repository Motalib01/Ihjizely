using System.Security.Claims;
using Ihjezly.Application.Reviews.CreateReview;
using Ihjezly.Application.Reviews.DeleteReview;
using Ihjezly.Application.Reviews.ListReviewsForProperty;
using Ihjezly.Application.Reviews.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ISender _mediator;

    public ReviewsController(ISender mediator) => _mediator = mediator;

    // POST: api/reviews
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var enrichedCommand = command with { UserId = parsedUserId };

        var result = await _mediator.Send(enrichedCommand);
        return Ok(result);
    }

    // GET: api/reviews/property/{id}
    [HttpGet("property/{id}")]
    public async Task<IActionResult> GetByPropertyId(Guid id) =>
        Ok(await _mediator.Send(new ListReviewsForPropertyQuery(id)));

    // DELETE: api/reviews/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        Ok(await _mediator.Send(new DeleteReviewCommand(id)));

    // PATCH: api/reviews/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewCommand command)
    {
        if (id != command.ReviewId)
            return BadRequest("Route ID and body ID must match.");

        return Ok(await _mediator.Send(command));
    }
}