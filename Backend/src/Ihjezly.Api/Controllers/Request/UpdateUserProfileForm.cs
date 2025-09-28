using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers.Request;

public class UpdateUserProfileForm
{
    [FromForm] public string FirstName { get; set; } = default!;
    [FromForm] public string LastName { get; set; } = default!;
    [FromForm] public string? PhoneNumber { get; set; } = default!;
    [FromForm] public string? Email { get; set; } = default!;
    [FromForm] public IFormFile? ProfileImageFile { get; set; }
}

