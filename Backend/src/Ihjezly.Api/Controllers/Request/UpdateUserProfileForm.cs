using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers.Request;

public class UpdateUserProfileForm
{
    [FromForm] public string FullName { get; set; } = default!;
    [FromForm] public string? PhoneNumber { get; set; } = default!;
    [FromForm] public string? Email { get; set; } = default!;
    [FromForm] public IFormFile? ProfileImageFile { get; set; }
}

