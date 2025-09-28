using Asp.Versioning;
using Ihjezly.Api.Controllers.Request;
using Ihjezly.Application.Users.CheckPhoneNumber;
using Ihjezly.Application.Users.DeleteUser;
using Ihjezly.Application.Users.ForgotPassword;
using Ihjezly.Application.Users.GetAllUsers;
using Ihjezly.Application.Users.GetCurrentUser;
using Ihjezly.Application.Users.GetUserById;
using Ihjezly.Application.Users.GetUserCountByRole;
using Ihjezly.Application.Users.LogInUser;
using Ihjezly.Application.Users.RegisterUser;
using Ihjezly.Application.Users.ReportUserViolation;
using Ihjezly.Application.Users.ResetPassword;
using Ihjezly.Application.Users.UpdateUser;
using Ihjezly.Application.Users.VerifyUser;
using Ihjezly.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator) => _mediator = mediator;

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var result = await _mediator.Send(new GetCurrentUserQuery(parsedUserId));
        return result.IsSuccess ? Ok(result.Value) : NotFound(ApiError.From(result.Error));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }

    [Authorize]
    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var command = new ResetPasswordCommand(
            parsedUserId,
            dto.OldPassword,
            dto.NewPassword,
            dto.ConfirmNewPassword
        );

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(ApiError.From(result.Error));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(ApiError.From(result.Error));
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(ApiError.From(result.Error));
    }

    [Authorize]
    [HttpPut("update-profile")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileForm form)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var command = new UpdateUserCommand(
            parsedUserId,
            form.FirstName,
            form.LastName,
            form.PhoneNumber,
            form.Email,
            form.ProfileImageFile
        );

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(ApiError.From(result.Error));
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(ApiError.From(result.Error));
    }

    [HttpGet("count-by-role")]
    public async Task<IActionResult> GetUserCountByRole([FromQuery] UserRole role)
    {
        var result = await _mediator.Send(new GetUserCountByRoleQuery(role));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(ApiError.From(result.Error));
    }

    [HttpPost("{id}/report-violation")]
    public async Task<IActionResult> ReportViolation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReportUserViolationCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockUser(Guid id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BlockUserCommand(id, reason), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id}/unblock")]
    public async Task<IActionResult> UnblockUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnblockUserCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : NotFound(ApiError.From(result.Error));
    }

    [HttpGet("check-phone")]
    public async Task<IActionResult> CheckPhone([FromQuery] string phoneNumber, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CheckPhoneNumberQuery(phoneNumber), cancellationToken);
        return Ok(new { exists = result });
    }
}