using Asp.Versioning;
using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Properties.Halls.GetHallPropertyById;
using Ihjezly.Application.Shared;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Claims;

namespace Ihjezly.Api.Controllers.Base;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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

            request.Images = uploadResult.Value
                .Select((url, index) => Image.Create(url, index == 0))
                .ToList();
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
        var uploadedImageUrls = new List<Image>();

        if (request.Images is not null && request.Images.Any())
        {
            var uploadResult = await _mediator.Send(new UploadFileCommand(request.Images, "properties"), cancellationToken);
            if (uploadResult.IsFailure)
                return BadRequest(uploadResult.Error);

            uploadedImageUrls = uploadResult.Value
                .Select((url, index) => Image.Create(url, index == 0))
                .ToList();
        }

        // Map DTO to command
        var command = request.ToCommand(id, uploadedImageUrls);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }



}