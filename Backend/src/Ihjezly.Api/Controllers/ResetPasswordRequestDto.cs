namespace Ihjezly.Api.Controllers;

public sealed record ResetPasswordRequestDto(
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
);