using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Properties.ChangePropertyStatus;
using Ihjezly.Application.Properties.DeleteProperty;
using Ihjezly.Application.Properties.GetAllPropertiesByType;
using Ihjezly.Application.Properties.GetPropertiesByStatus;
using Ihjezly.Application.Properties.GetPropertyById;
using Ihjezly.Application.Shared;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ihjezly.Api.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class PropertyControllerBase<TProperty, TDetails> : ControllerBase
    where TProperty : PropertyWithDetails<TDetails>, new()
{
    private readonly ISender _mediator;
    private readonly IWebHostEnvironment _environment;

    protected PropertyControllerBase(ISender mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPropertyByIdQuery<Apartment, ApartmentDetails>(id));

        if (result.IsFailure)
            return NotFound(new { result.Error.Code, result.Error.Message });

        return Ok(result.Value); 
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreatePropertyRequest<TProperty, TDetails> request,
        [FromForm] List<IFormFile>? images)
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var businessOwnerId))
            return Unauthorized("Invalid or missing user ID.");

        request.BusinessOwnerId = businessOwnerId;

        if (images is not null && images.Any())
        {
            var uploadResult = await _mediator.Send(new UploadFileCommand(images, "properties"));
            if (uploadResult.IsFailure)
                return BadRequest(uploadResult.Error);

            request.Images = uploadResult.Value;
        }

        var result = await _mediator.Send(request.ToCommand());

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }




    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdatePropertyRequest<TProperty, TDetails> request,
        [FromForm] List<IFormFile>? newImages)
    {
        // 1. Ensure route ID and body ID match
        if (id != request.PropertyId)
            return BadRequest("Route ID and body ID must match.");

        // 2. Get authenticated user's ID
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var businessOwnerId))
            return Unauthorized("Invalid or missing user ID.");

        request.BusinessOwnerId = businessOwnerId;

        // 3. Upload new images if any
        if (newImages is { Count: > 0 })
        {
            var uploadResult = await _mediator.Send(new UploadFileCommand(newImages, "properties"));
            if (uploadResult.IsFailure)
                return BadRequest(uploadResult.Error);

            request.NewImages = uploadResult.Value;
        }

        // 4. Delete removed images from disk
        if (request.DeletedImages is { Count: > 0 })
        {
            foreach (var imageUrl in request.DeletedImages)
            {
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "images", "properties", fileName);

                try
                {
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    // Log this if necessary, but don't block the request
                    // _logger.LogWarning(ex, "Failed to delete image {filePath}", filePath);
                }
            }
        }

        // 5. Send update command to handler
        var result = await _mediator.Send(request.ToCommand());

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePropertyCommand<TProperty, TDetails>(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllPropertiesByTypeQuery<TProperty, TDetails>());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetAllByStatus(string status)
    {
        if (!Enum.TryParse<PropertyStatus>(status, true, out var parsedStatus))
            return BadRequest("Invalid status");

        var result = await _mediator.Send(new GetAllPropertiesByTypeAndStatusQuery<TProperty, TDetails>(parsedStatus));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] PropertyStatus newStatus)
    {
        var result = await _mediator.Send(new ChangePropertyStatusCommand(id, newStatus));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

}