using Ihjezly.Application.Shared;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Claims;
using Ihjezly.Application.Properties.Halls.GetHallPropertyById;
using Ihjezly.Api.Controllers.Request;

namespace Ihjezly.Api.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class HallPropertyControllerBase<TProperty, TDetails> : ControllerBase
    where TProperty : HallProperty<TDetails>, new()
{
    private readonly ISender _mediator;
    private readonly IWebHostEnvironment _environment;

    protected HallPropertyControllerBase(ISender mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetHallPropertyByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateHallPropertyRequest<TProperty, TDetails> request, [FromForm] List<IFormFile>? images)
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

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdateHallPropertyRequest<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var uploadedImageUrls = new List<string>();

        // Upload new images
        if (request.Images is not null && request.Images.Any())
        {
            var uploadResult = await _mediator.Send(new UploadFileCommand(request.Images, "properties"), cancellationToken);

            if (uploadResult.IsFailure)
                return BadRequest(uploadResult.Error);

            uploadedImageUrls.AddRange(uploadResult.Value);
        }

        // Map DTO to command
        var command = request.ToCommand(id, uploadedImageUrls);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }



}