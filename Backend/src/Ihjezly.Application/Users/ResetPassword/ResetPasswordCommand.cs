using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.ResetPassword;

public sealed record ResetPasswordCommand(
    Guid UserId,
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
) : ICommand;
